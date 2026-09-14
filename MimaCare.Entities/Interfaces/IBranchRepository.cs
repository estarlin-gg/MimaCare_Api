using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MimaCare.Domain.Interfaces
{
    public interface IBranchRepository
    {
        Task<IEnumerable<Branch>> GetByClinicAsync(Guid clinicId);

        Task<Branch?> GetByIdAsync(Guid id);

        Task<int> CountByClinicAsync(Guid clinicId);

        Task CreateAsync(Branch branch);

        Task UpdateAsync(Branch branch);

        Task DeleteAsync(Branch branch);

        Task<bool> HasAppointmentsAsync(Guid branchId);

        Task SaveChangesAsync();
    }
}