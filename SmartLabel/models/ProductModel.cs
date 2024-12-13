using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartLabel.models
{
    public class ProductModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
        public int? Discount { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int CategoryId { get; set; }
    }
}
