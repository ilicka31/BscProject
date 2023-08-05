using AutoMapper;
using Common.Entities.Product;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Consumers;
using OrderService.Database;
using OrderService.Mapping;
using OrderService.Repository;
using OrderService.Services;
using RabbitMQ.Client;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:44370",
            IssuerSigningKey = key
        };
    });
builder.Services.AddDbContext<OrderDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("Orders"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

builder.Services.AddMassTransit(x => {
    x.AddConsumer<UserInfoConsumer>();
    x.AddConsumer<ProductInfoConsumer>();
    x.AddConsumer<ProductDeleteConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://localhost:5672"), h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("user-created-order", e =>
        {
            e.ConfigureConsumer<UserInfoConsumer>(context);
        });
        cfg.ReceiveEndpoint("product-created-order", e =>
        {
            e.ConfigureConsumer<ProductInfoConsumer>(context);
        });
        cfg.ReceiveEndpoint("product-deleted-order", e =>
        {
            e.ConfigureConsumer<ProductDeleteConsumer>(context);
        });
    });
});

/*
var factory = new ConnectionFactory { Uri = new Uri("amqp://localhost:5672") };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);

builder.Services.AddSingleton(channel);
builder.Services.AddHostedService<UserInfoConsumer>();
*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
