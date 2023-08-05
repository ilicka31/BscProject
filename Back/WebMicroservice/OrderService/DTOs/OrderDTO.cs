using System.ComponentModel.DataAnnotations;

namespace OrderService.DTOs
{
    public class OrderDTO
    {
        [Required]
        public string Comment { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public double TotalPrice { get; set; }
        [Required]
        public bool IsPayed { get; set; }
    }
}
