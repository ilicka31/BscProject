using Common.Entites;
using Common.Entities.Product;
using MassTransit;
using OrderService.Database;

namespace OrderService.Consumers
{
    public class ProductDeleteConsumer : IConsumer<ProductDeleted>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ProductDeleteConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            // _dbContext = dbContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(ConsumeContext<ProductDeleted> context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<OrderDBContext>();

                var newPD = new ProductDeleted
                {
                    Id = context.Message.Id,
                    SellerId = context.Message.SellerId
                };
                var product = _dbContext.ProductInfos.Find(context.Message.Id);
                if (product != null)
                {
                    _dbContext.ProductInfos.Remove(product);
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
