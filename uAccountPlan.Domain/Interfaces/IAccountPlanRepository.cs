using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using uAccountPlan.Domain.Entities;

namespace uAccountPlan.Domain.Interfaces
{
    public interface IAccountPlanRepository
    {
        Task<IEnumerable<AccountPlan>> GetAllAsync();
        Task<AccountPlan?> GetByIdAsync(Guid id);
        Task AddAsync(AccountPlan accountPlan);
        Task DeleteAsync(Guid id);
        Task<List<AccountPlan>> GetChildrenAsync(Guid parentId);
    }
}