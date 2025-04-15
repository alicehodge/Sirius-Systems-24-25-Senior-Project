using StorkDorkMain.Models;
using System.ComponentModel.DataAnnotations;

namespace StorkDorkMain.Models
{
    public class RangeSubmission : ModeratedContent
    {
        [Required]
        public int BirdId { get; set; }
        public virtual Bird Bird { get; set; }

        [Required(ErrorMessage = "Please provide range information")]
        [MinLength(5, ErrorMessage = "Range description should be at least 5 characters")] 
        [MaxLength(2000, ErrorMessage = "Range description cannot exceed 2000 characters")]
        public string RangeDescription { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string SubmissionNotes { get; set; }
    }
}