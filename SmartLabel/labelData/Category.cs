using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartLabel.labelData
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Descount { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImagePath { get; set; }
    }
}
