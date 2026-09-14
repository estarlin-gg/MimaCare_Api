using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Branch;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/doctors/{doctorId:guid}/branches")]
    [ApiController]
    [Authorize(Roles = "Clinic,Super")]
    public class DoctorBranchController : BaseController
    {
        private readonly IBranchService _branchService;

        public DoctorBranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorBranches(Guid doctorId)
        {
            var branches = await _branchService.GetDoctorBranchesAsync(doctorId);
            return Success(branches);
        }

        [HttpPut]
        public async Task<IActionResult> SetDoctorBranches(Guid doctorId, AssignDoctorBranchesDto dto)
        {
            var message = await _branchService.SetDoctorBranchesAsync(doctorId, dto);
            return Success(message);
        }
    }
}