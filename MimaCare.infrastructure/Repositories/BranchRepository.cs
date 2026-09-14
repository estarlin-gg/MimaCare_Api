using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;

namespace MimaCare.Infrastructure.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        private readonly MimaCareContext _context;

        public BranchRepository(MimaCareContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Branch>> GetByClinicAsync(Guid clinicId)
        {
            return await _context.Branches
                .Where(b => b.ClinicId == clinicId)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Branch?> GetByIdAsync(Guid id)
        {
            return await _context.Branches
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<int> CountByClinicAsync(Guid clinicId)
        {
            return await _context.Branches
                .CountAsync(b => b.ClinicId == clinicId);
        }

        public async Task CreateAsync(Branch branch)
        {
            await _context.Branches.AddAsync(branch);
        }

        public Task UpdateAsync(Branch branch)
        {
            _context.Branches.Update(branch);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Branch branch)
        {
            _context.Branches.Remove(branch);
            return Task.CompletedTask;
        }

        public async Task<bool> HasAppointmentsAsync(Guid branchId)
        {
            return await _context.Appointments
                .AnyAsync(a => a.BranchId == branchId || a.CreatedBranchId == branchId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}