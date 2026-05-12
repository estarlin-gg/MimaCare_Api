using MimaCare.Infrastructure.Constants;

namespace MimaCare.Application.Dtos.Auth
{
    public class RegisterClinicDto
    {


        public string ClinicName { get; set; } = default!;

        public string ClinicEmail { get; set; } = default!;

        public string ClinicPhone { get; set; } = default!;

        public string ClinicAddress { get; set; } = default!;

        // =====================================================
        // Admin User
        // =====================================================

        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}
