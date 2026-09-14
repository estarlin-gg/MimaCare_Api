using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Infrastructure.Context;

namespace MimaCare.Infrastructure.Seed
{
    public static class SubscriptionPlanSeeder
    {
        private static readonly Guid BasicId = new Guid("c3a00000-0000-4000-8000-000000000001");
        private static readonly Guid ProfessionalId = new Guid("c3a00000-0000-4000-8000-000000000002");
        private static readonly Guid EnterpriseId = new Guid("c3a00000-0000-4000-8000-000000000003");

        public static async Task SeedAsync(MimaCareContext context)
        {
            // Ensure the 3 standard plans exist (idempotent by fixed id).
            var basic = await context.SubscriptionPlans.FindAsync(BasicId);
            var professional = await context.SubscriptionPlans.FindAsync(ProfessionalId);
            var enterprise = await context.SubscriptionPlans.FindAsync(EnterpriseId);

            if (basic == null)
            {
                basic = new SubscriptionPlan
                {
                    Id = BasicId,
                    Name = "Básico",
                    Description = "1 sucursal y hasta 3 doctores, citas ilimitadas.",
                    MaxBranches = 1,
                    MaxDoctors = 3,
                    AllowsUnlimitedAppointments = true,
                    IsActive = true
                };
                context.SubscriptionPlans.Add(basic);
            }

            if (professional == null)
            {
                professional = new SubscriptionPlan
                {
                    Id = ProfessionalId,
                    Name = "Profesional",
                    Description = "Hasta 3 sucursales y doctores ilimitados, citas ilimitadas.",
                    MaxBranches = 3,
                    MaxDoctors = null,
                    AllowsUnlimitedAppointments = true,
                    IsActive = true
                };
                context.SubscriptionPlans.Add(professional);
            }

            if (enterprise == null)
            {
                enterprise = new SubscriptionPlan
                {
                    Id = EnterpriseId,
                    Name = "Enterprise",
                    Description = "Sucursales y doctores ilimitados, citas ilimitadas, soporte custom.",
                    MaxBranches = null,
                    MaxDoctors = null,
                    AllowsUnlimitedAppointments = true,
                    IsActive = true
                };
                context.SubscriptionPlans.Add(enterprise);
            }

            // Keep define of every existing clinic: any clinic without a plan
            // gets the Básico plan, except known demo clinics (assigned later).
            var clinics = await context.Clinics
                .Include(c => c.Branches)
                .ToListAsync();

            // The demo clinic (mimacare@gmail.com) requires the Enterprise plan.
            var mimacareDemoClinicId =
                (await context.Users
                    .FirstOrDefaultAsync(u => u.Email == "mimacare@gmail.com"))
                    ?.ClinicId;

            foreach (var clinic in clinics)
            {
                if (clinic.PlanId == null)
                {
                    clinic.PlanId = BasicId;
                }

                if (mimacareDemoClinicId != null && clinic.Id == mimacareDemoClinicId.Value)
                {
                    clinic.PlanId = EnterpriseId;
                }
            }

            // Ensure every clinic has at least one default branch.
            foreach (var clinic in clinics)
            {
                if (!clinic.Branches.Any())
                {
                    context.Branches.Add(new Branch
                    {
                        Id = Guid.NewGuid(),
                        ClinicId = clinic.Id,
                        Name = "Sucursal Principal",
                        IsActive = true
                    });
                }
            }

            await context.SaveChangesAsync();

            // Assign every doctor to their clinic's first/default branch.
            var branchesByClinic = await context.Branches
                .ToListAsync();

            var doctors = await context.Doctors.ToListAsync();

            foreach (var doctor in doctors)
            {
                var defaultBranch = branchesByClinic
                    .Where(b => b.ClinicId == doctor.ClinicId)
                    .OrderBy(b => b.CreatedAt)
                    .FirstOrDefault();

                if (defaultBranch == null)
                {
                    continue;
                }

                var alreadyLinked = await context.DoctorBranches
                    .AnyAsync(db => db.DoctorId == doctor.Id && db.BranchId == defaultBranch.Id);

                if (!alreadyLinked)
                {
                    context.DoctorBranches.Add(new DoctorBranch
                    {
                        DoctorId = doctor.Id,
                        BranchId = defaultBranch.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}