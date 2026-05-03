using System.ComponentModel.DataAnnotations;

namespace NaskoCuts.Models.Entities
{
    public class Barber
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        public string Role { get; set; } = string.Empty;

        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string InstagramUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}