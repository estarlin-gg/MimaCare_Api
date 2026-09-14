using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;

namespace MimaCare.Infrastructure.Repositories
{
    public class DoctorBranchRepository : IDoctorBranchRepository
    {
        private readonly MimaCareContext _context;

        public DoctorBranchRepository(MimaCareContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetBranchesByDoctorAsync(Guid doctorId)
        {
            return await _context.DoctorBranches
                .Where(db => db.DoctorId == doctorId)
                .Select(db => db.Branch)
                .ToListAsync();
        }

        public async Task<IEnumerable<Guid>> GetBranchIdsByDoctorAsync(Guid doctorId)
        {
            return await _context.DoctorBranches
                .Where(db => db.DoctorId == doctorId)
                .Select(db => db.BranchId)
                .ToListAsync();
        }

        public async Task<bool> DoctorWorksInBranchAsync(Guid doctorId, Guid branchId)
        {
            return await _context.DoctorBranches
                .AnyAsync(db => db.DoctorId == doctorId && db.BranchId == branchId);
        }

        public async Task DeleteAllForDoctorAsync(Guid doctorId)
        {
            var rows = await _context.DoctorBranches
                .Where(db => db.DoctorId == doctorId)
                .ToListAsync();

            _context.DoctorBranches.RemoveRange(rows);
        }

        public async Task CreateAsync(DoctorBranch doctorBranch)
        {
            await _context.DoctorBranches.AddAsync(doctorBranch);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}