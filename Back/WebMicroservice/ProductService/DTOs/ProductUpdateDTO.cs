namespace ProductService.DTOs
{
    public class ProductUpdateDTO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int MaxQuantity { get; set; }
    }
}
