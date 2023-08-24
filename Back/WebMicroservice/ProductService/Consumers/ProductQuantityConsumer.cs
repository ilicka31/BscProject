using Common.Entities.Order;
using MassTransit;
using ProductService.Database;

namespace ProductService.Consumers
{
    public class ProductQuantityConsumer : IConsumer<ProductQuantity>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ProductQuantityConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(ConsumeContext<ProductQuantity> context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<ProductDBContext>();
                var product = _dbContext.Products.Find(context.Message.Id);

                /*var pq = new ProductQuantity() { 
                    Id = context.Message.Id,
                    Quantity = context.Message.Quantity
                };*/
                product.MaxQuantity -= context.Message.Quantity;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
