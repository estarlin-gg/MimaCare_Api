namespace MimaCare.Application.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string? FullName { get; set; }

        public string? ClinicName { get; set; }

        public string? Role { get; set; }

        public string? Token { get; set; }

        public string Message { get; set; } = default!;
    }
}
