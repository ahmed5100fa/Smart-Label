using System.ComponentModel.DataAnnotations.Schema;

namespace SmartLabel.models
{
    public class CategoryModel
    {
        public string Name { get; set; }
        
        public IFormFile? Image { get; set; }
      
    }
}
