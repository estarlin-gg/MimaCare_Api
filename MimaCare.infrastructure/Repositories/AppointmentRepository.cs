using Microsoft.EntityFrameworkCore;
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
            var appointment = await _repository.GetByIdAsync(id);

            return appointment;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByClinicId(Guid clinicId)
        {
            var res = await _context.Appointments.Where(a => a.ClinicId == clinicId).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorId(Guid doctorId)
        {
            var res = await _context.Appointments.Where(a => a.DoctorId == doctorId).ToListAsync();

            return res;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(Guid patientId)
        {
            var res = await _context.Appointments.Where(a => a.PatientId == patientId).ToListAsync();

            return res;
        }
    }
}
