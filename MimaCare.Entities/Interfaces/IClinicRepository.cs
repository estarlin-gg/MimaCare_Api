using MimaCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MimaCare.Infrastructure.Repositories.Abstraction
{
    public interface IClinicRepository
    {
        Task CreateClinic(Clinic clinic);
    }
}
