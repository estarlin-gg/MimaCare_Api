using MimaCare.Application.Dtos.Appointment;
using MimaCare.Application.Dtos.Common;
using MimaCare.Domain.Common;

namespace MimaCare.Application.Interface
{
    public interface IAppointmentService
    {
        Task<PagedResult<AppointmentDto>> GetAppointmentsByClinic(PaginationParams pagination);
        Task<PagedResult<AppointmentDto>> GetMyAppointments(PaginationParams pagination);
        Task<AppointmentDto> GetAppointmentById(Guid id);
        Task<PagedResult<AppointmentDto>> GetAppointmentsByDoctorId(Guid doctorId, PaginationParams pagination);
        Task<PagedResult<AppointmentDto>> GetAppointmentsByPatientId(Guid patientId, PaginationParams pagination);
        Task<AppointmentDto> CreateAppointment(CreateAppointmentDto dto);
        Task<AppointmentDto> UpdateAppointment(Guid id, UpdateAppointmentDto dto);
        Task<AppointmentDto> UpdateAppointmentStatus(Guid id, UpdateAppointmentStatusDto dto);
        Task<AppointmentDto> AssignDoctor(Guid id, AssignDoctorDto dto);
    }
}
