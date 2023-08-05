using Common.Entites;
using Common.Entities.Product;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using OrderService.Models;

namespace OrderService.Repository
{
    public class OrderRepository : IOrderRepository
    {
        OrderDBContext _dbContext;

        public OrderRepository(OrderDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Order> Create(Order entity)
        {
            await _dbContext.Orders.AddAsync(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public Task Delete(long id)
        {
            _dbContext.Orders.Remove(_dbContext.Orders.Find(id));
            return _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            List<Order> orders = await _dbContext.Orders.ToListAsync();
            return orders;
        }

        public async Task<List<ProductInfo>> GetAllProducts()
        {
         return  await _dbContext.ProductInfos.ToListAsync();
        }

        public async Task<List<UserInfo>> GetAllUsers()
        {
            return await _dbContext.UserInfos.ToListAsync();
        }

        public async Task<Order> GetById(long id)
        {
            return await _dbContext.Orders.FindAsync(id);
        }

        public async Task<long> GetNewestOrderId()
        {
            var orders = await _dbContext.Orders.OrderByDescending(e => e.Id).ToListAsync();
            return orders.FirstOrDefault().Id;
        }

        public async Task<List<Order>> GetSellerOrders(long sellerId, bool old)
        {
            List<Order> orders = await _dbContext.Orders.Where(o => o.Confirmed &&
                                (old ? o.DeliveryDate < DateTime.Now : o.DeliveryDate > DateTime.Now))
                                //.Include(x => x.OrderItems.Where(oi => oi.Article.SellerId == sellerId))
                                //.ThenInclude(x => x.Article)
                                .ToListAsync();

            return orders;
        }

        public async Task<UserInfo> GetUser(long id)
        {
            return await _dbContext.UserInfos.FindAsync(id);
        }

        public Task Update(Order entity)
        {
            Order newOrder = _dbContext.Orders.Find(entity.Id);
            newOrder = entity;
            _dbContext.Orders.Update(newOrder);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateProduct(ProductInfo product)
        {
            ProductInfo productInfo = _dbContext.ProductInfos.Find(product.Id);
            productInfo = product;
            _dbContext.ProductInfos.Update(productInfo);
            return _dbContext.SaveChangesAsync();
        }
    }
}
