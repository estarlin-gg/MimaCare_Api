using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task CreateAppointment(Appointment appointment);

        Task<Appointment?> GetAppointmentById(Guid id);

        Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(Guid patientId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorId(Guid doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByClinicId(Guid clinicId);



        


    }
}
