using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using uAccountPlan.Application.DTOs;
using uAccountPlan.Application.Services;
using uAccountPlan.Domain.Entities;
using uAccountPlan.Domain.Interfaces;

namespace uAccountPlan.Tests.Services
{
    public class CriacaoPlanoContaServiceTests
    {
        private readonly Mock<IAccountPlanRepository> _repositoryMock;
        private readonly AccountPlanService _service;

        public CriacaoPlanoContaServiceTests()
        {
            _repositoryMock = new Mock<IAccountPlanRepository>();
            _service = new AccountPlanService(_repositoryMock.Object);
        }

        [Fact(DisplayName = "Deve lançar erro se o pai aceita lançamentos")]
        public async Task NaoDevePermitirCriarFilhoSePaiAceitaLancamentos()
        {
            var pai = new AccountPlan { Id = Guid.NewGuid(), AcceptsLaunches = true, Type = "Receita" };
            _repositoryMock.Setup(r => r.GetByIdAsync(pai.Id)).ReturnsAsync(pai);

            var dto = new AccountPlanDto
            {
                Code = "1.1.1",
                Name = "Teste",
                Type = "Receita",
                AcceptsLaunches = true,
                ParentId = pai.Id
            };

            var act = async () => await _service.AddAsync(dto);
            await act.Should().ThrowAsync<Exception>().WithMessage("A conta que aceita lançamentos não pode ter contas filhas.");
        }

        [Fact(DisplayName = "Deve lançar erro se o tipo for diferente do pai")]
        public async Task NaoDevePermitirTipoDiferenteDoPai()
        {
            var pai = new AccountPlan { Id = Guid.NewGuid(), AcceptsLaunches = false, Type = "Despesa" };
            _repositoryMock.Setup(r => r.GetByIdAsync(pai.Id)).ReturnsAsync(pai);

            var dto = new AccountPlanDto
            {
                Code = "1.1.2",
                Name = "Teste",
                Type = "Receita",
                AcceptsLaunches = true,
                ParentId = pai.Id
            };

            var act = async () => await _service.AddAsync(dto);
            await act.Should().ThrowAsync<Exception>().WithMessage("As contas devem obrigatoriamente ser do mesmo tipo do seu pai.");
        }

        [Fact(DisplayName = "Deve lançar erro se o código já existir")]
        public async Task NaoDevePermitirCodigoDuplicado()
        {
            var existente = new AccountPlan { Code = "1.2.3" };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AccountPlan> { existente });

            var dto = new AccountPlanDto
            {
                Code = "1.2.3",
                Name = "Duplicado",
                Type = "Receita",
                AcceptsLaunches = true,
                ParentId = null
            };

            var act = async () => await _service.AddAsync(dto);
            await act.Should().ThrowAsync<Exception>().WithMessage("Os códigos não podem se repetir.");
        }

        [Fact(DisplayName = "Deve permitir código fora de ordem como 1.2.9")]
        public async Task DevePermitirCodigoForaDeOrdem()
        {
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<AccountPlan>());

            var dto = new AccountPlanDto
            {
                Code = "1.2.9",
                Name = "Livre",
                Type = "Receita",
                AcceptsLaunches = true,
                ParentId = null
            };

            var act = async () => await _service.AddAsync(dto);
            await act.Should().NotThrowAsync();
        }
    }
}