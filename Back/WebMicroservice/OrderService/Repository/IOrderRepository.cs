using Common;
using Common.Entites;
using Common.Entities.Product;
using OrderService.Models;

namespace OrderService.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<List<Order>> GetSellerOrders(long sellerId, bool old);
        Task<long> GetNewestOrderId();
        Task<List<ProductInfo>> GetAllProducts();
        Task UpdateProduct(ProductInfo product);
        Task<UserInfo> GetUser(long id);
        Task<List<UserInfo>> GetAllUsers();
    }
}
