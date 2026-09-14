using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;

namespace MimaCare.Domain.Interfaces
{
    public interface IDoctorBranchRepository
    {
        Task<IEnumerable<Branch>> GetBranchesByDoctorAsync(Guid doctorId);

        Task<IEnumerable<Guid>> GetBranchIdsByDoctorAsync(Guid doctorId);

        Task<bool> DoctorWorksInBranchAsync(Guid doctorId, Guid branchId);

        Task DeleteAllForDoctorAsync(Guid doctorId);

        Task CreateAsync(DoctorBranch doctorBranch);

        Task SaveChangesAsync();
    }
}