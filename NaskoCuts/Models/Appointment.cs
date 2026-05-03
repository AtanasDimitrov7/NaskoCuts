using System.ComponentModel.DataAnnotations;

namespace NaskoCuts.Models.Entities
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100)]
        public string ClientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имейлът е задължителен")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        public string ClientEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефонът е задължителен")]
        [Phone(ErrorMessage = "Невалиден телефонен номер")]
        public string ClientPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Датата е задължителна")]
        public DateTime AppointmentDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public string ConfirmationCode { get; set; } = string.Empty;

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        [Required]
        public int BarberId { get; set; }
        public Barber Barber { get; set; } = null!;
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
}