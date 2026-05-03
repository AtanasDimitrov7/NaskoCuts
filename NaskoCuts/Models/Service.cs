using System.ComponentModel.DataAnnotations;

namespace NaskoCuts.Models.Entities
{
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Цената е задължителна")]
        [Range(0, 1000, ErrorMessage = "Цената трябва да е между 0 и 1000")]
        public decimal Price { get; set; }

        [Required]
        public int DurationMinutes { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}