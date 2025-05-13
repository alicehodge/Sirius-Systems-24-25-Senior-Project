using System.ComponentModel.DataAnnotations;

namespace StorkDorkMain.Models
{
    public class BirdPhoto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int BirdId { get; set; }
        public virtual Bird Bird { get; set; }

        [Required]
        public byte[] PhotoData { get; set; }

        [Required]
        public string PhotoContentType { get; set; }

        // Optional: Let user add a caption or notes
        [MaxLength(255)]
        public string? Caption { get; set; }
        public DateTime DateAdded { get; internal set; }
    }
}