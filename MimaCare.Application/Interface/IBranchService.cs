using MimaCare.Application.Dtos.Branch;
using System;
using System.Collections.Generic;

namespace MimaCare.Application.Interface
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetMyBranchesAsync();

        Task<BranchDto> CreateBranchAsync(CreateBranchDto dto);

        Task<BranchDto> UpdateBranchAsync(Guid branchId, UpdateBranchDto dto);

        Task<string> DeleteBranchAsync(Guid branchId);

        Task<IEnumerable<BranchDto>> GetDoctorBranchesAsync(Guid doctorId);

        Task<IEnumerable<Guid>> GetDoctorBranchIdsAsync(Guid doctorId);

        Task<string> SetDoctorBranchesAsync(Guid doctorId, AssignDoctorBranchesDto dto);
    }
}