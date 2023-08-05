using Common.Entites;
using Common.Entities.Product;
using MassTransit;
using OrderService.Database;

namespace OrderService.Consumers
{
    public class ProductInfoConsumer : IConsumer<ProductInfo>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ProductInfoConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            // _dbContext = dbContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(ConsumeContext<ProductInfo> context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<OrderDBContext>();

                var newPI = new ProductInfo
                {
                    Id= context.Message.Id,
                    Price = context.Message.Price,
                    MaxQuantity = context.Message.MaxQuantity,
                    Name = context.Message.Name,
                    SellerId = context.Message.SellerId
                };
                var product = _dbContext.ProductInfos.Find(context.Message.Id);
                if (product == null)
                {
                    _dbContext.ProductInfos.Add(newPI);
                }
                else
                {
                    product = newPI;
                    _dbContext.ProductInfos.Update(product);
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
