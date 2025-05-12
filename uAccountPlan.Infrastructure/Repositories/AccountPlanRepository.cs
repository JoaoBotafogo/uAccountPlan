using Microsoft.EntityFrameworkCore;
using uAccountPlan.Domain.Entities;
using uAccountPlan.Domain.Interfaces;

namespace uAccountPlan.Infrastructure.Repositories
{
    public class AccountPlanRepository : IAccountPlanRepository
    {
        private readonly AppDbContext _context;

        public AccountPlanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AccountPlan>> GetAllAsync()
        {
            return await _context.AccountPlans.ToListAsync();
        }

        public async Task<AccountPlan?> GetByIdAsync(Guid id)
        {
            return await _context.AccountPlans.FindAsync(id);
        }

        public async Task AddAsync(AccountPlan accountPlan)
        {
            await _context.AccountPlans.AddAsync(accountPlan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var accountPlan = await GetByIdAsync(id);
            if (accountPlan != null)
            {
                _context.AccountPlans.Remove(accountPlan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<AccountPlan>> GetChildrenAsync(Guid parentId)
        {
            return await _context.AccountPlans
                .Where(ap => ap.ParentId == parentId)
                .ToListAsync();
        }
    }
}