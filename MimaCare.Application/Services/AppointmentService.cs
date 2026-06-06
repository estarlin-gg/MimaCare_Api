using MimaCare.Application.Dtos.Appointment;
using MimaCare.Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        public Task CreateAppointment(CreateAppointmentDto appointment)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentDto> GetAppointmentById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicId(Guid clinicId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorId(Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientId(Guid doctorId)
        {
            throw new NotImplementedException();
        }
    }
}
