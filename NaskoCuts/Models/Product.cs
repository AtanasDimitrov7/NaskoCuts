using System.ComponentModel.DataAnnotations;

namespace NaskoCuts.Models.Entities
{
    public enum ProductCategory
    {
        Hair,
        Beard,
        Skin,
        Tools
    }

    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public ProductCategory Category { get; set; }

        public int Stock { get; set; }

        public bool IsActive { get; set; } = true;
    }
}