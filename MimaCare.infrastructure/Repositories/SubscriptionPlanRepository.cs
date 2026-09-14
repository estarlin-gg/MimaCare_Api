using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;

namespace MimaCare.Infrastructure.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly MimaCareContext _context;

        public SubscriptionPlanRepository(MimaCareContext context)
        {
            _context = context;
        }

        public async Task<SubscriptionPlan?> GetByIdAsync(Guid id)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<SubscriptionPlan?> GetByNameAsync(string name)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Name == name);
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllActiveAsync()
        {
            return await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task CreateAsync(SubscriptionPlan plan)
        {
            await _context.SubscriptionPlans.AddAsync(plan);
        }

        public Task UpdateAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}