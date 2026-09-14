using MimaCare.Application.Dtos.SubscriptionPlan;
using System;
using System.Collections.Generic;

namespace MimaCare.Application.Interface
{
    public interface ISubscriptionPlanService
    {
        Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync();

        Task<PlanUsageDto> GetMyPlanUsageAsync();

        Task<PlanUsageDto> GetClinicPlanUsageAsync(Guid clinicId);

        Task<string> CreatePlanAsync(CreateSubscriptionPlanDto dto);

        Task<string> UpdatePlanAsync(Guid planId, UpdateSubscriptionPlanDto dto);

        Task<string> AssignPlanToClinicAsync(Guid clinicId, AssignPlanToClinicDto dto);
    }
}