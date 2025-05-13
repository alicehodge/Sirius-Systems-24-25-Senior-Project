//// filepath: /Users/alicehodge/Documents/GitHub/Sirius-Systems-24-25-Senior-Project/storkdork/StorkDorkMain/ViewModels/BirdPhotoSubmissionViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace StorkDorkMain.Models
{
    public class BirdPhotoSubmissionViewModel
    {
        [Required]
        public int BirdId { get; set; }

        // If user is uploading a file
        public IFormFile Photo { get; set; }

        // You can store file bytes from Photo before submission if desired
        public byte[] PhotoData { get; set; }

        public string PhotoContentType { get; set; }

        [MaxLength(255)]
        public string Caption { get; set; }
    }
}