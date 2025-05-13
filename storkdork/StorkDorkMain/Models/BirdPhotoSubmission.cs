using System.ComponentModel.DataAnnotations;

namespace StorkDorkMain.Models
{
    public class BirdPhotoSubmission : ModeratedContent
    {
        [Required]
        public int BirdId { get; set; }
        public virtual Bird Bird { get; set; }
        
        public byte[] PhotoData { get; set; }
        public string PhotoContentType { get; set; }
        
        // Optional: Let user add a caption or notes
        [MaxLength(255)]
        public string? Caption { get; set; }
        
        public BirdPhotoSubmission()
        {
            ContentType = "BirdPhoto";
        }
    }
}