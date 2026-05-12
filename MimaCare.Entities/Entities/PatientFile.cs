namespace MimaCare.Domain.Entities
{
    public class PatientFile
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }

        public Patient Patient { get; set; }
            = default!;

        public string UploadedByUserId { get; set; }
            = default!;

        public ApplicationUser UploadedByUser { get; set; }
            = default!;

        public string FileName { get; set; }
            = default!;

        public string FileUrl { get; set; }
            = default!;

        public string ContentType { get; set; }
            = default!;

        public DateTime UploadedAt { get; set; }
            = DateTime.UtcNow;
    }
}
