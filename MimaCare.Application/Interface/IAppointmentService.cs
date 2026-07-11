using MimaCare.Application.Dtos.Appointment;

namespace MimaCare.Application.Interface
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByClinic();
        Task<IEnumerable<AppointmentDto>> GetMyAppointments();
        Task<AppointmentDto> GetAppointmentById(Guid id);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorId(Guid doctorId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientId(Guid patientId);
        Task<AppointmentDto> CreateAppointment(CreateAppointmentDto dto);
        Task<AppointmentDto> UpdateAppointmentStatus(Guid id, UpdateAppointmentStatusDto dto);
        Task<AppointmentDto> AssignDoctor(Guid id, AssignDoctorDto dto);
    }
}
