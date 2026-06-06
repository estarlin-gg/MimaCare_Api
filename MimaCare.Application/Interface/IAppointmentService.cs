using MimaCare.Application.Dtos.Appointment;

namespace MimaCare.Application.Interface
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinicId(Guid clinicId);

        Task<AppointmentDto> GetAppointmentById(Guid id);

        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorId(Guid doctorId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientId(Guid doctorId);

        Task CreateAppointment(CreateAppointmentDto appointment);


    }
}
