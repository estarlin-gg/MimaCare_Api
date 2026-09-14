using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.Branch;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/Branch")]
    [ApiController]
    [Authorize(Roles = "Clinic,Super")]
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyBranches()
        {
            var branches = await _branchService.GetMyBranchesAsync();
            return Success(branches);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch(CreateBranchDto dto)
        {
            var branch = await _branchService.CreateBranchAsync(dto);
            return CreatedResponse(branch, "Sucursal creada correctamente.");
        }

        [HttpPut("{branchId:guid}")]
        public async Task<IActionResult> UpdateBranch(Guid branchId, UpdateBranchDto dto)
        {
            var branch = await _branchService.UpdateBranchAsync(branchId, dto);
            return Success(branch, "Sucursal actualizada correctamente.");
        }

        [HttpDelete("{branchId:guid}")]
        public async Task<IActionResult> DeleteBranch(Guid branchId)
        {
            var message = await _branchService.DeleteBranchAsync(branchId);
            return Success(message);
        }
    }
}