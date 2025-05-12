using uAccountPlan.Application.DTOs;
using uAccountPlan.Domain.Entities;

namespace uAccountPlan.Application.Interfaces;

public interface IAccountPlanService
{
    Task<IEnumerable<AccountPlanDto>> GetAllAsync();
    Task<AccountPlan?> GetByIdAsync(Guid id);
    Task AddAsync(AccountPlanDto accountPlan);
    Task DeleteAsync(Guid id);
    Task<SuggestNextCodeResult> SuggestNextCodeAsync(Guid parentId);
}