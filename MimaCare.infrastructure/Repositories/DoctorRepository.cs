using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Common;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Infrastructure.Repositories
{
    public class DoctorRepository : IDoctorRepository

    {
        private readonly MimaCareContext _mimaCareContext;
        private readonly IRepository<Doctor> _repository;

        public DoctorRepository(IRepository<Doctor> repository, MimaCareContext mimaCareContext)
        {
            _repository = repository;
            _mimaCareContext = mimaCareContext;
        }
        public async Task CreateDoctor(Doctor doctor)
        {
            await _repository.AddAsync(doctor);

        }

        public async Task<Doctor?> GetDoctorById(Guid doctorId)
        {
            var doctor = await _mimaCareContext.Doctors
                .Include(u => u.User)
                .ThenInclude(c => c.Clinic)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            return doctor;
        }

        public async Task<Doctor?> GetDoctorByUserId(Guid userId)
        {
            var doctor = await _mimaCareContext.Doctors
                .Include(u => u.User)
                .ThenInclude(c => c.Clinic)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            return doctor;
        }

        public async Task<PagedResult<Doctor>> GetDoctors(Guid clinicId, int page, int pageSize, string? search = null)
        {
            var query = _mimaCareContext.Doctors
               .Include(u => u.User)
               .ThenInclude(c => c.Clinic)
               .Where(d => d.ClinicId == clinicId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                var pattern = $"%{term}%";

                query = query.Where(d =>
                    EF.Functions.ILike(d.User.FirstName, pattern) ||
                    EF.Functions.ILike(d.User.LastName, pattern) ||
                    EF.Functions.ILike(d.Specialty, pattern) ||
                    EF.Functions.ILike(d.User.Email!, pattern) ||
                    EF.Functions.ILike(d.User.PhoneNumber!, pattern));
            }

            var totalCount = await query.CountAsync();

            var items = await query
               .OrderBy(d => d.User.FirstName)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            return new PagedResult<Doctor>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task UpdateDoctor(Doctor doctor)
        {
            _repository.Update(doctor);

            await Task.CompletedTask;
        }

        public async Task DeleteDoctor(Doctor doctor)
        {
            _repository.Delete(doctor);

            await Task.CompletedTask;
        }


    }
}
