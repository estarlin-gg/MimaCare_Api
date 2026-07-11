using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Infrastructure.Repositories
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly IRepository<DoctorSchedule> _repository;
        private readonly MimaCareContext _context;

        public DoctorScheduleRepository(IRepository<DoctorSchedule> repository, MimaCareContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task CreateSchedule(DoctorSchedule schedule)
            => await _repository.AddAsync(schedule);

        public Task UpdateSchedule(DoctorSchedule schedule)
        {
            _repository.Update(schedule);
            return Task.CompletedTask;
        }

        public Task DeleteSchedule(DoctorSchedule schedule)
        {
            _repository.Delete(schedule);
            return Task.CompletedTask;
        }

        public async Task<DoctorSchedule?> GetScheduleById(Guid id)
            => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorId(Guid doctorId)
            => await _context.DoctorSchedules
                .Where(s => s.DoctorId == doctorId)
                .OrderBy(s => s.DayOfWeek)
                .ToListAsync();

        public async Task<DoctorSchedule?> GetScheduleByDoctorAndDay(Guid doctorId, DayOfWeek dayOfWeek)
            => await _context.DoctorSchedules
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek);

        public async Task SaveChangesAsync()
            => await _repository.SaveChangesAsync();
    }
}
