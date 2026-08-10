using MimaCare.Domain.Entities;

namespace MimaCare.Infrastructure.Repositories.Abstraction
{
    public interface IClinicRepository
    {
        Task CreateClinic(Clinic clinic);

        Task<Clinic?> GetByIdAsync(Guid id);
    }
}
