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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser -> Clinic

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.Clinic)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.ClinicId);

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
        }
    }
}
