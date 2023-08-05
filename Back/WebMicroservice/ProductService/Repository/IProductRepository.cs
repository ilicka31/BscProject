using Common;
using Common.Entites;
using ProductService.Models;

namespace ProductService.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetSellerProducts(long id);
        Task<byte[]> GetProductImage(long id);
        Task<UserInfo> GetSeller(long id);
    }
}
