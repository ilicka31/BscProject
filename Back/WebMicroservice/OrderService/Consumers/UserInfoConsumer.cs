using Common.Entites;
using MassTransit;
using OrderService.Database;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderService.Consumers
{
    public class UserInfoConsumer :IConsumer<UserInfo>
    {
        //private readonly OrderDBContext _dbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserInfoConsumer( IServiceScopeFactory serviceScopeFactory)
        {
           // _dbContext = dbContext;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(ConsumeContext<UserInfo> context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<OrderDBContext>();

                var newUI = new UserInfo
                {
                    Address = context.Message.Address,
                    Approved = context.Message.Approved,
                    Id = context.Message.Id,
                    Email = context.Message.Email,
                    Name = context.Message.Name,
                    UserType = context.Message.UserType,
                };
                var user = _dbContext.UserInfos.Find(context.Message.Id);
                if (user == null)
                {
                    _dbContext.UserInfos.Add(newUI);
                }
                else
                {
                    user = newUI;
                    _dbContext.UserInfos.Update(user);
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
