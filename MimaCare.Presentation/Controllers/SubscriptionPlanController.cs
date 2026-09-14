using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimaCare.Application.Dtos.SubscriptionPlan;
using MimaCare.Application.Interface;
using MimaCare.Presentation.Common;

namespace MimaCare.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Clinic,Super")]
    public class SubscriptionPlanController : BaseController
    {
        private readonly ISubscriptionPlanService _subscriptionPlanService;

        public SubscriptionPlanController(ISubscriptionPlanService subscriptionPlanService)
        {
            _subscriptionPlanService = subscriptionPlanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _subscriptionPlanService.GetAllPlansAsync();
            return Success(plans);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyPlanUsage()
        {
            var usage = await _subscriptionPlanService.GetMyPlanUsageAsync();
            return Success(usage);
        }

        [HttpGet("clinic/{clinicId:guid}/usage")]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> GetClinicPlanUsage(Guid clinicId)
        {
            var usage = await _subscriptionPlanService.GetClinicPlanUsageAsync(clinicId);
            return Success(usage);
        }

        [HttpPost]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> CreatePlan(CreateSubscriptionPlanDto dto)
        {
            var message = await _subscriptionPlanService.CreatePlanAsync(dto);
            return CreatedResponse(message);
        }

        [HttpPut("{planId:guid}")]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> UpdatePlan(Guid planId, UpdateSubscriptionPlanDto dto)
        {
            var message = await _subscriptionPlanService.UpdatePlanAsync(planId, dto);
            return Success(message);
        }

        [HttpPut("{clinicId:guid}/assign")]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> AssignPlanToClinic(Guid clinicId, AssignPlanToClinicDto dto)
        {
            var message = await _subscriptionPlanService.AssignPlanToClinicAsync(clinicId, dto);
            return Success(message);
        }
    }
}