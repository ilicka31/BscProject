using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Values;
using ProductService.Consumers;
using ProductService.Database;
using ProductService.Mapping;
using ProductService.Repository;
using ProductService.Services;
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
builder.Services.AddDbContext<ProductDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("Products"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
//, ServiceLifetime.Scoped
var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();


builder.Services.AddMassTransit(x => {
    x.AddConsumer<UserCreatedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri("rabbitmq://localhost:5672"), h => {
            h.Username("guest");
            h.Password("guest");
        });
       
        cfg.ReceiveEndpoint("user-created-product", e =>
        {
            e.ConfigureConsumer<UserCreatedConsumer>(context);
       });
    
    });

});


/*
var factory = new ConnectionFactory { Uri = new Uri("amqp://localhost:5672") };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);

builder.Services.AddSingleton(channel);
builder.Services.AddHostedService<UserCreatedConsumer>();
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
