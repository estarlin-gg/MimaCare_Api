namespace MimaCare.Application.Dtos.Branch
{
    public class CreateBranchDto
    {
        public string Name { get; set; } = default!;

        public string? Address { get; set; }

        public string? Phone { get; set; }
    }
}