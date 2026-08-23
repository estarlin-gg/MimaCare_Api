using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Common;
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

        public async Task<PagedResult<Patient>> GetPatients(
            Guid clinicId,
            int page,
            int pageSize,
            string? search = null)
        {
            var query = _context
                .Patients
                .Include(x => x.Clinic)
                .Where(x =>
                    x.ClinicId == clinicId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";

                query = query.Where(x =>
                    EF.Functions.ILike(x.FirstName, pattern) ||
                    EF.Functions.ILike(x.LastName, pattern) ||
                    EF.Functions.ILike(x.Phone, pattern) ||
                    EF.Functions.ILike(x.Address, pattern));
            }

            var totalCount =
                await query.CountAsync();

            var items = await query
                .OrderBy(x =>
                    x.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Patient>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
