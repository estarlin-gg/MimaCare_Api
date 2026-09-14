using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MimaCare.Domain.Interfaces
{
    public interface ISubscriptionPlanRepository
    {
        Task<SubscriptionPlan?> GetByIdAsync(Guid id);

        Task<SubscriptionPlan?> GetByNameAsync(string name);

        Task<IEnumerable<SubscriptionPlan>> GetAllActiveAsync();

        Task CreateAsync(SubscriptionPlan plan);

        Task UpdateAsync(SubscriptionPlan plan);

        Task SaveChangesAsync();
    }
}