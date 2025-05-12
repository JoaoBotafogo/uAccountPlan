using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using uAccountPlan.Application.Services;
using uAccountPlan.Domain.Entities;
using uAccountPlan.Domain.Interfaces;

namespace uAccountPlan.Tests.Services
{
    public class SugestaoCodigoServiceTests
    {
        private readonly Mock<IAccountPlanRepository> _repositoryMock;
        private readonly AccountPlanService _service;

        public SugestaoCodigoServiceTests()
        {
            _repositoryMock = new Mock<IAccountPlanRepository>();
            _service = new AccountPlanService(_repositoryMock.Object);
        }

        [Fact(DisplayName = "Deve sugerir próximo código corretamente")]
        public async Task DeveSugerirProximoCodigoCorretamente()
        {
            var pai = new AccountPlan { Id = Guid.NewGuid(), Code = "1.2", AcceptsLaunches = false };

            var filhos = new List<AccountPlan>
            {
                new AccountPlan { Code = "1.2.1" },
                new AccountPlan { Code = "1.2.3" },
                new AccountPlan { Code = "1.2.2" }
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(pai.Id)).ReturnsAsync(pai);
            _repositoryMock.Setup(r => r.GetChildrenAsync(pai.Id)).ReturnsAsync(filhos);

            var resultado = await _service.SuggestNextCodeAsync(pai.Id);

            resultado.SuggestedCode.Should().Be("1.2.4");
            resultado.SuggestedParentId.Should().Be(pai.Id);
        }

        [Fact(DisplayName = "Deve subir para o pai do pai se código for maior que 999")]
        public async Task DeveSubirParaPaiDoPaiSePassarLimite()
        {
            var avo = new AccountPlan { Id = Guid.NewGuid(), Code = "1", AcceptsLaunches = false };
            var pai = new AccountPlan { Id = Guid.NewGuid(), Code = "1.2", AcceptsLaunches = false, ParentId = avo.Id };

            var filhos = Enumerable.Range(1, 999).Select(i => new AccountPlan { Code = $"1.2.{i}" }).ToList();

            _repositoryMock.Setup(r => r.GetByIdAsync(pai.Id)).ReturnsAsync(pai);
            _repositoryMock.Setup(r => r.GetChildrenAsync(pai.Id)).ReturnsAsync(filhos);
            _repositoryMock.Setup(r => r.GetByIdAsync(avo.Id)).ReturnsAsync(avo);
            _repositoryMock.Setup(r => r.GetChildrenAsync(avo.Id)).ReturnsAsync(new List<AccountPlan>());

            var resultado = await _service.SuggestNextCodeAsync(pai.Id);

            resultado.SuggestedCode.Should().Be("1.1");
            resultado.SuggestedParentId.Should().Be(avo.Id);
        }
    }
}