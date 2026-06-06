using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IRepository<Patient>
         _repository;

        private readonly MimaCareContext
            _context;

        public PatientRepository(
            IRepository<Patient> repository,
            MimaCareContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task CreatePatient(
            Patient patient)
        {
            await _repository
                .AddAsync(patient);
        }

        public async Task UpdatePatient(
            Patient patient)
        {
            _repository.Update(patient);

            await Task.CompletedTask;
        }

        public async Task DeletePatient(
            Patient patient)
        {
            _repository.Delete(patient);

            await Task.CompletedTask;
        }

        public async Task<Patient?> GetPatientById(
            Guid patientId,
            Guid clinicId)
        {
            return await _context
                .Patients
                .Include(x => x.Clinic)
                .Include(x => x.Appointments)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x =>
                    x.Id == patientId &&
                    x.ClinicId == clinicId);
        }

        public async Task<IEnumerable<Patient>> GetPatients(
            Guid clinicId)
        {
            return await _context
                .Patients
                .Include(x => x.Clinic)
                .Where(x =>
                    x.ClinicId == clinicId)
                .OrderBy(x =>
                    x.FirstName)
                .ToListAsync();
        }
    }
}
