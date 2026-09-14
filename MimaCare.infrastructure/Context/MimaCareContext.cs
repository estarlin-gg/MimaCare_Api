using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;

namespace MimaCare.Infrastructure.Context
{
    public class MimaCareContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public MimaCareContext(DbContextOptions<MimaCareContext> options) : base(options)
        {

        }
        public DbSet<Clinic> Clinics { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<PatientFile> PatientFiles { get; set; }

        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }

        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<DoctorSpecialty> DoctorSpecialties { get; set; }

        public DbSet<RevokedToken> RevokedTokens { get; set; }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

        public DbSet<Branch> Branches { get; set; }

        public DbSet<DoctorBranch> DoctorBranches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser -> Clinic

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.ClinicId);

            // Clinic -> SubscriptionPlan

            builder.Entity<Clinic>()
                .HasOne(x => x.Plan)
                .WithMany(x => x.Clinics)
                .HasForeignKey(x => x.PlanId)
                .IsRequired(false);

            // Doctor

            builder.Entity<Doctor>()
                .HasOne(x => x.User)
                .WithOne(x => x.Doctor)
                .HasForeignKey<Doctor>(x => x.UserId);

            builder.Entity<Doctor>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Doctors)
                .HasForeignKey(x => x.ClinicId);


            // Patient

            builder.Entity<Patient>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Patients)
                .HasForeignKey(x => x.ClinicId);


            // Appointment

            builder.Entity<Appointment>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.PatientId);

            builder.Entity<Appointment>()
                .HasOne(x => x.Doctor)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.DoctorId);

            builder.Entity<Appointment>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.ClinicId);

            // PatientFile
            builder.Entity<PatientFile>()
                .HasOne(x => x.Patient)
                .WithMany(x => x.Files)
                .HasForeignKey(x => x.PatientId);

            builder.Entity<PatientFile>()
                .HasOne(x => x.UploadedByUser)
                .WithMany(x => x.UploadedFiles)
                .HasForeignKey(x => x.UploadedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // DoctorSchedule
            builder.Entity<DoctorSchedule>()
                .HasOne(x => x.Doctor)
                .WithMany(x => x.Schedules)
                .HasForeignKey(x => x.DoctorId);

            builder.Entity<DoctorSchedule>()
                .Property(x => x.DayOfWeek)
                .HasConversion<int>();

            // Branch

            builder.Entity<Branch>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Branches)
                .HasForeignKey(x => x.ClinicId);

            // DoctorBranch (many-to-many doctor <-> branch)
            builder.Entity<DoctorBranch>()
                .HasKey(x => new { x.DoctorId, x.BranchId });

            builder.Entity<DoctorBranch>()
                .HasOne(x => x.Doctor)
                .WithMany(x => x.DoctorBranches)
                .HasForeignKey(x => x.DoctorId);

            builder.Entity<DoctorBranch>()
                .HasOne(x => x.Branch)
                .WithMany(x => x.DoctorBranches)
                .HasForeignKey(x => x.BranchId);

            // Appointment -> Branch (attended) and CreatedBranch (created)
            builder.Entity<Appointment>()
                .HasOne(x => x.Branch)
                .WithMany(x => x.AttendedAppointments)
                .HasForeignKey(x => x.BranchId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(x => x.CreatedBranch)
                .WithMany(x => x.CreatedAppointments)
                .HasForeignKey(x => x.CreatedBranchId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // DoctorSpecialty (many-to-many)
            builder.Entity<DoctorSpecialty>()
                .HasKey(x => new { x.DoctorId, x.SpecialtyId });

            builder.Entity<DoctorSpecialty>()
                .HasOne(x => x.Doctor)
                .WithMany(x => x.DoctorSpecialties)
                .HasForeignKey(x => x.DoctorId);

            builder.Entity<DoctorSpecialty>()
                .HasOne(x => x.Specialty)
                .WithMany(x => x.DoctorSpecialties)
                .HasForeignKey(x => x.SpecialtyId);

            // RevokedToken (JWT logout blacklist)
            builder.Entity<RevokedToken>()
                .HasIndex(x => x.Jti)
                .IsUnique();

            // Subscription plans (fixed ids so seeder/registration stay idempotent)
            builder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan
                {
                    Id = new Guid("c3a00000-0000-4000-8000-000000000001"),
                    Name = "Básico",
                    Description = "1 sucursal y hasta 3 doctores, citas ilimitadas.",
                    MaxBranches = 1,
                    MaxDoctors = 3,
                    AllowsUnlimitedAppointments = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new SubscriptionPlan
                {
                    Id = new Guid("c3a00000-0000-4000-8000-000000000002"),
                    Name = "Profesional",
                    Description = "Hasta 3 sucursales y doctores ilimitados, citas ilimitadas.",
                    MaxBranches = 3,
                    MaxDoctors = null,
                    AllowsUnlimitedAppointments = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 1, DateTimeKind.Utc)
                },
                new SubscriptionPlan
                {
                    Id = new Guid("c3a00000-0000-4000-8000-000000000003"),
                    Name = "Enterprise",
                    Description = "Sucursales y doctores ilimitados, citas ilimitadas, soporte custom.",
                    MaxBranches = null,
                    MaxDoctors = null,
                    AllowsUnlimitedAppointments = true,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 2, DateTimeKind.Utc)
                }
            );
        }
    }
}
