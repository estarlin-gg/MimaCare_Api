namespace MimaCare.Application.Dtos.Auth
{
    public class RegisterClinicDto
    {
        public string ClinicName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string? ClinicPhone { get; set; }

        public string? ClinicAddress { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        // Subscription plan chosen at registration. When null, the clinic
        // is assigned the default plan (Básico).
        public Guid? PlanId { get; set; }
    }
}
