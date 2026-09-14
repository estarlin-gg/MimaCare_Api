namespace MimaCare.Application.Dtos.Branch
{
    public class UpdateBranchDto
    {
        public string Name { get; set; } = default!;

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}