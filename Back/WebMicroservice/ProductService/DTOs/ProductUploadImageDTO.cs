namespace ProductService.DTOs
{
    public class ProductUploadImageDTO
    {
        public long Id { get; set; }
        public IFormFile File { get; set; }
    }
}
