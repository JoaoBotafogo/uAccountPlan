using Xunit;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using uAccountPlan.Application.Services;
using uAccountPlan.Domain.Entities;
using uAccountPlan.Domain.Interfaces;
using uAccountPlan.Application.Commands;
using uAccountPlan.Domain.Enums;

public class AccountPlanServiceTests
{
    private readonly IAccountPlanRepository _repositorio;
    private readonly AccountPlanService _servico;

    public AccountPlanServiceTests()
    {
        _repositorio = Substitute.For<IAccountPlanRepository>();
        _servico = new AccountPlanService(_repositorio);
    }

    // -------------------------
    // REGRA DE ADDASYNC
    // -------------------------

    [Fact]
    public async Task Nao_Deve_Permitir_Criar_Conta_Filha_Se_Pai_Aceita_Lancamentos()
    {
        var pai = new AccountPlan { Id = Guid.NewGuid(), AcceptsLaunches = true };
        _repositorio.GetByIdAsync(pai.Id).Returns(pai);

        var comando = new AddAccountPlanCommand
        {
            ParentId = pai.Id,
            Code = "1.2.1",
            Type = AccountType.Receita,
            AcceptsLaunches = true
        };

        Func<Task> acao = async () => await _servico.AddAsync(comando);

        await acao.Should().ThrowAsync<Exception>()
            .WithMessage("A conta que aceita lançamentos não pode ter contas filhas.");
    }

    [Fact]
    public async Task Nao_Deve_Permitir_Codigo_Repetido()
    {
        _repositorio.CodeExistsAsync("1.2.3").Returns(true);

        var comando = new AddAccountPlanCommand
        {
            Code = "1.2.3",
            Type = AccountType.Receita,
            AcceptsLaunches = true
        };

        Func<Task> acao = async () => await _servico.AddAsync(comando);

        await acao.Should().ThrowAsync<Exception>()
            .WithMessage("Já existe uma conta com esse código.");
    }

    [Fact]
    public async Task Nao_Deve_Permitir_Tipo_Diferente_Do_Pai()
    {
        var pai = new AccountPlan { Id = Guid.NewGuid(), Type = AccountType.Receita };
        _repositorio.GetByIdAsync(pai.Id).Returns(pai);

        var comando = new AddAccountPlanCommand
        {
            ParentId = pai.Id,
            Code = "1.2.3",
            Type = AccountType.Despesa,
            AcceptsLaunches = true
        };

        Func<Task> acao = async () => await _servico.AddAsync(comando);

        await acao.Should().ThrowAsync<Exception>()
            .WithMessage("O tipo da conta deve ser o mesmo do pai.");
    }

    [Fact]
    public async Task Deve_Permitir_Codigo_Menor_Que_O_Maior_Existente()
    {
        var pai = new AccountPlan { Id = Guid.NewGuid(), Code = "1.2", Type = AccountType.Receita };
        _repositorio.GetByIdAsync(pai.Id).Returns(pai);
        _repositorio.CodeExistsAsync("1.2.9").Returns(false);

        var comando = new AddAccountPlanCommand
        {
            ParentId = pai.Id,
            Code = "1.2.9",
            Type = AccountType.Receita,
            AcceptsLaunches = true
        };

        var result = await _servico.AddAsync(comando);

        result.Code.Should().Be("1.2.9");
    }

    // -------------------------
    // REGRA DE SUGESTÃO DE CÓDIGO
    // -------------------------

    [Fact]
    public async Task Deve_Sugerir_Proximo_Codigo_Filho_Incrementando_Maior()
    {
        var pai = new AccountPlan { Id = Guid.NewGuid(), Code = "2.2" };
        var filhos = new List<AccountPlan>
        {
            new AccountPlan { Code = "2.2.5" },
            new AccountPlan { Code = "2.2.7" }
        };

        _repositorio.GetByIdAsync(pai.Id).Returns(pai);
        _repositorio.GetChildrenAsync(pai.Id).Returns(filhos);

        var sugestao = await _servico.SuggestNextCodeAsync(pai.Id);

        sugestao.SuggestedCode.Should().Be("2.2.8");
        sugestao.SuggestedParentId.Should().Be(pai.Id);
    }

    [Fact]
    public async Task Deve_Sugerir_Codigo_No_Pai_Avô_Se_Limite_999_Atingido()
    {
        var neto = new AccountPlan
        {
            Id = Guid.NewGuid(),
            Code = "1.2",
            ParentId = Guid.NewGuid()
        };
        var avo = new AccountPlan
        {
            Id = neto.ParentId.Value,
            Code = "1"
        };

        var filhosDoNeto = new List<AccountPlan>();
        for (int i = 1; i <= 999; i++)
            filhosDoNeto.Add(new AccountPlan { Code = $"1.2.{i}" });

        _repositorio.GetByIdAsync(neto.Id).Returns(neto);
        _repositorio.GetChildrenAsync(neto.Id).Returns(filhosDoNeto);
        _repositorio.GetByIdAsync(avo.Id).Returns(avo);
        _repositorio.GetChildrenAsync(avo.Id).Returns(new List<AccountPlan> { new AccountPlan { Code = "1.1" } });

        var sugestao = await _servico.SuggestNextCodeAsync(neto.Id);

        sugestao.SuggestedCode.Should().Be("1.2"); // O próximo disponível do avô será "1.2"
        sugestao.SuggestedParentId.Should().Be(avo.Id);
    }

    [Fact]
    public async Task Deve_Lancar_Erro_Se_Nao_Puder_Subir_Mais_Niveis()
    {
        var pai = new AccountPlan
        {
            Id = Guid.NewGuid(),
            Code = "9",
            ParentId = null
        };
        var filhos = new List<AccountPlan>();
        for (int i = 1; i <= 999; i++)
            filhos.Add(new AccountPlan { Code = $"9.{i}" });

        _repositorio.GetByIdAsync(pai.Id).Returns(pai);
        _repositorio.GetChildrenAsync(pai.Id).Returns(filhos);

        Func<Task> acao = async () => await _servico.SuggestNextCodeAsync(pai.Id);

        await acao.Should().ThrowAsync<Exception>()
            .WithMessage("Não é possível sugerir novo código. Limite máximo atingido.");
    }
}