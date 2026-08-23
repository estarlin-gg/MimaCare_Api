using Microsoft.EntityFrameworkCore;
using MimaCare.Domain.Common;
using MimaCare.Domain.Entities;
using MimaCare.Domain.Interfaces;
using MimaCare.Infrastructure.Context;
using MimaCare.Infrastructure.Repositories.Abstraction;

namespace MimaCare.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IRepository<Appointment> _repository;
 
        private readonly MimaCareContext _context;


        public AppointmentRepository(IRepository<Appointment> repository, MimaCareContext context)
        {
            _repository = repository;
            _context = context;
        }
        public async Task CreateAppointment(Appointment appointment)
        {
            await _repository.AddAsync(appointment);
        }

        public async Task<Appointment?> GetAppointmentById(Guid id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            return appointment;
        }

        public async Task<PagedResult<Appointment>> GetAppointmentsByClinicId(Guid clinicId, int page, int pageSize)
        {
            var query = _context.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Where(a => a.ClinicId == clinicId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Appointment>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorId(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<PagedResult<Appointment>> GetAppointmentsByDoctorId(Guid doctorId, int page, int pageSize)
        {
            var query = _context.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Where(a => a.DoctorId == doctorId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Appointment>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<Appointment>> GetAppointmentsByPatientId(Guid patientId, int page, int pageSize)
        {
            var query = _context.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ThenInclude(d => d.User)
                .Where(a => a.PatientId == patientId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Appointment>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
