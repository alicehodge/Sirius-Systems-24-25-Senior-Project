using System;

namespace StorkDorkMain.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }  // "Success", "Warning", "Info"
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? RelatedUrl { get; set; }

        public virtual SdUser User { get; set; }
    }
}