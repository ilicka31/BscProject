using Common;
using ProductService.DTOs;
using ProductService.Models;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<List<ProductGetDTO>> GetAllProducts();
        Task<List<ProductGetDTO>> GetSellerProducts(long id);
        Task<ProductDTO> GetProduct(long id, long userId);
        Task<ProductDTO> AddNewProduct(ProductDTO newProduct, long sellerId);
        Task<bool> DeleteProduct(long id);
        Task<ProductUpdateDTO> UpdateProduct(long id, ProductUpdateDTO newProduct);
        Task UploadImage(long id, IFormFile file);
        Task<ProductImageDTO> GetProductImage(long id);
        Task<Product> GetProduct(long id);
        Task<Product> GetLast(long userId);

    }
}
