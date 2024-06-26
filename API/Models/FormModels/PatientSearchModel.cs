using System.ComponentModel.DataAnnotations;

namespace API.Models.FormModels
{
    public class PatientSearchModel
    {
        public string? Query { get; set; }

        public string Type { get; set; } = "NIC";
    }

    public class PatientModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Nic { get; set; }

        [Required]
        public string? BloodGroup { get; set; }

        [Required]
        public DateOnly dateOfBirth { get; set; }

        [Required]
        public float Height { get; set; }

        [Required]
        public float Weight { get; set; }

        public string? Address { get; set; }

        [Required]
        public string Gender { get; set; }

        // Add the profile image
        public IFormFile? profileImage { get; set; }
    }
}
