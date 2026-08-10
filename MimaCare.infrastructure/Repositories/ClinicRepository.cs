using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Infrastructure.Context;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Infrastructure.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly MimaCareContext _context;

        public ClinicRepository(MimaCareContext context)
        {
            _context = context;
        }

        public async Task CreateClinic(Clinic clinic)
        {
            await _context.Clinics.AddAsync(clinic);
        }

        public async Task<Clinic?> GetByIdAsync(Guid id)
        {
            return await _context.Clinics.FindAsync(id);
        }
    }
}
