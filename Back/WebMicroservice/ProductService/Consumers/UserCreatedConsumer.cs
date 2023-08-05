using Common.Entites;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Database;
using ProductService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ProductService.Consumers
{
        public class UserCreatedConsumer : IConsumer<UserInfo>
        {
        //   private readonly ProductDBContext _dbContext;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserCreatedConsumer(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /*  public UserCreatedConsumer(ProductDBContext dbContext)
          {
              _dbContext = dbContext;
          }*/

        public async Task Consume(ConsumeContext<UserInfo> context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _dbContext = scope.ServiceProvider.GetRequiredService<ProductDBContext>();
                var newUI = new UserInfo
                {
                    Address = context.Message.Address,
                    Approved = context.Message.Approved,
                    Id = context.Message.Id,
                    Email = context.Message.Email,
                    Name = context.Message.Name,
                    UserType = context.Message.UserType,
                };
                var user = _dbContext.UsersInfo.Find(context.Message.Id);
                if (user == null)
                {

                    _dbContext.UsersInfo.Add(newUI);
                }
                else
                {
                    user = newUI;
                    _dbContext.UsersInfo.Update(user);
                }
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}

