using Microsoft.AspNetCore.Identity;

namespace StorkDorkMain.Models
{
    public class ModeratedContent
    {
        public int Id { get; set; }
        public string ContentType { get; set; } // e.g., "BirdRegion"
        public int ContentId { get; set; }
        public int SubmitterId { get; set; }
        public virtual SdUser Submitter { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        public int? ModeratorId { get; set; }
        public virtual SdUser Moderator { get; set; }
        public DateTime? ModeratedDate { get; set; }
        public string? ModeratorNotes { get; set; }
    }

    public enum ModerationType
    {
        Pending,
        Approved,
        Rejected
    }
}