using System.ComponentModel.DataAnnotations;

namespace NaskoCuts.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Съдържанието е задължително")]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Range(1, 5, ErrorMessage = "Оценката трябва да е между 1 и 5")]
        public int Rating { get; set; } = 5;

        public string AvatarUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsApproved { get; set; } = false;
    }
}