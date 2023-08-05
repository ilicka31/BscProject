using System.ComponentModel.DataAnnotations;

namespace ProductService.DTOs
{
    public class ProductDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public int MaxQuantity { get; set; }
        [Required]
        public IFormFile FormFile { get; set; }
    }
}
