using uAccountPlan.Application.DTOs;
using uAccountPlan.Application.Interfaces;
using uAccountPlan.Domain.Entities;
using uAccountPlan.Domain.Interfaces;

namespace uAccountPlan.Application.Services
{
    public class AccountPlanService : IAccountPlanService
    {
        private readonly IAccountPlanRepository _repository;

        public AccountPlanService(IAccountPlanRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AccountPlanDto>> GetAllAsync()
        {
            var accountPlans = await _repository.GetAllAsync();
            return accountPlans
                .OrderBy(a => a.Code) 
                .Select(a => new AccountPlanDto
                {
                    Code = a.Code,
                    Name = a.Name,
                    Type = a.Type,
                    AcceptsLaunches = a.AcceptsLaunches,
                    ParentId = a.ParentId
                });
        }

        public async Task<AccountPlan?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(AccountPlanDto accountPlanDto)
        {
            var accountPlan = new AccountPlan
            {
                Id = Guid.NewGuid(), 
                Code = accountPlanDto.Code,
                Name = accountPlanDto.Name,
                Type = accountPlanDto.Type,
                AcceptsLaunches = accountPlanDto.AcceptsLaunches,
                ParentId = accountPlanDto.ParentId
            };

            if (accountPlan.ParentId != null)
            {
                var parent = await _repository.GetByIdAsync(accountPlan.ParentId.Value);
                if (parent == null)
                {
                    throw new Exception("Conta pai não encontrada.");
                }

                if (parent.AcceptsLaunches)
                {
                    throw new Exception("A conta que aceita lançamentos não pode ter contas filhas.");
                }

                if (parent.Type != accountPlan.Type)
                {
                    throw new Exception("As contas devem obrigatoriamente ser do mesmo tipo do seu pai.");
                }
            }

            var existingAccount = (await _repository.GetAllAsync()).FirstOrDefault(a => a.Code == accountPlan.Code);
            if (existingAccount != null)
            {
                throw new Exception("Os códigos não podem se repetir.");
            }

            await _repository.AddAsync(accountPlan);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
        public async Task<SuggestNextCodeResult> SuggestNextCodeAsync(Guid parentId)
        {
            var parent = await _repository.GetByIdAsync(parentId);
            if (parent == null)
                throw new Exception("Conta pai não encontrada.");

            var children = await _repository.GetChildrenAsync(parentId);

            var codes = children
                .Select(c => c.Code)
                .Where(c => c.StartsWith(parent.Code + "."))
                .Select(c => c.Substring(parent.Code.Length + 1))
                .Where(c => int.TryParse(c, out _))
                .Select(int.Parse)
                .ToList();

            int next = codes.Any() ? codes.Max() + 1 : 1;

            if (next > 999)
            {
                if (parent.ParentId == null)
                    throw new Exception("Não é possível sugerir novo código. Limite máximo atingido.");

                return await SuggestNextCodeAsync(parent.ParentId.Value);
            }

            var suggestedCode = $"{parent.Code}.{next}";

            return new SuggestNextCodeResult
            {
                SuggestedCode = suggestedCode,
                SuggestedParentId = parent.Id
            };
        }
    }
}