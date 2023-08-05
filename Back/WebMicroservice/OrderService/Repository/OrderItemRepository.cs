using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using OrderService.Models;

namespace OrderService.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly OrderDBContext _context;

        public OrderItemRepository(OrderDBContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> Create(OrderItem entity)
        {
            await _context.OrderItems.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public Task Delete(long id)
        {
            _context.OrderItems.Remove(_context.OrderItems.Find(id));
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetAll()
        {
            return await _context.OrderItems.ToListAsync();
        }

        public async Task<OrderItem> GetById(long id)
        {
            return await _context.OrderItems.FindAsync(id);
        }

        public Task Update(OrderItem entity)
        {
            OrderItem newOrderItem = _context.OrderItems.Find(entity.Id);
            newOrderItem = entity;
            _context.OrderItems.Update(newOrderItem);
            return _context.SaveChangesAsync();
        }
    }
}
