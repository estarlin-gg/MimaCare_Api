using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
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
            //var doctor = await _repository.GetByIdAsync(doctorId);

            var doctor = await _mimaCareContext.Doctors
                .Include(u => u.User)
                .ThenInclude(c => c.Clinic)
                .FirstOrDefaultAsync(d => d.UserId == doctorId);

            return doctor;
        }

        public async Task<IEnumerable<Doctor>> GetDoctors(Guid clinicId)
        {
            //var doctors = await _repository.FindAsync(x => x.ClinicId == clinicId);
            var doctors = await _mimaCareContext.Doctors
               .Include(u => u.User)
               .ThenInclude(c => c.Clinic)
               .Where(d => d.ClinicId == clinicId)
               .ToListAsync();

            return doctors;
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
