using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Infrastructure.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly IRepository<Specialty> _repository;
        private readonly MimaCareContext _context;

        public SpecialtyRepository(IRepository<Specialty> repository, MimaCareContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task CreateSpecialty(Specialty specialty)
            => await _repository.AddAsync(specialty);

        public Task UpdateSpecialty(Specialty specialty)
        {
            _repository.Update(specialty);
            return Task.CompletedTask;
        }

        public Task DeleteSpecialty(Specialty specialty)
        {
            _repository.Delete(specialty);
            return Task.CompletedTask;
        }

        public async Task<Specialty?> GetSpecialtyById(Guid id)
            => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Specialty>> GetAllSpecialties()
            => await _context.Specialties
                .OrderBy(s => s.Name)
                .ToListAsync();

        public async Task<IEnumerable<Specialty>> GetSpecialtiesByDoctorId(Guid doctorId)
            => await _context.DoctorSpecialties
                .Where(ds => ds.DoctorId == doctorId)
                .Select(ds => ds.Specialty)
                .OrderBy(s => s.Name)
                .ToListAsync();

        public async Task<IEnumerable<DoctorSpecialty>> GetDoctorSpecialtyRelations(Guid doctorId)
            => await _context.DoctorSpecialties
                .Where(ds => ds.DoctorId == doctorId)
                .ToListAsync();

        public async Task AddDoctorSpecialty(DoctorSpecialty relation)
            => await _context.DoctorSpecialties.AddAsync(relation);

        public Task DeleteDoctorSpecialty(DoctorSpecialty relation)
        {
            _context.DoctorSpecialties.Remove(relation);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
            => await _repository.SaveChangesAsync();
    }
}
