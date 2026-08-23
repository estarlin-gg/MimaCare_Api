using MimaCare.Domain.Common;
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

        Task<PagedResult<Appointment>> GetAppointmentsByPatientId(Guid patientId, int page, int pageSize);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorId(Guid doctorId);
        Task<PagedResult<Appointment>> GetAppointmentsByDoctorId(Guid doctorId, int page, int pageSize);
        Task<PagedResult<Appointment>> GetAppointmentsByClinicId(Guid clinicId, int page, int pageSize);
    }
}
