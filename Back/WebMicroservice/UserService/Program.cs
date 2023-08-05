using Microsoft.EntityFrameworkCore;
using UserService.Database;
using UserService.Services.EmailService;
using UserService.Services;
using AutoMapper;
using UserService.Mapping;
using UserService.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
//string _cors = "cors";
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserService", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

});


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



builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<UserDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("Users"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});
/*
var factory = new ConnectionFactory { Uri = new Uri("amqp://localhost:5672") };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "fanout_exchange", type: ExchangeType.Fanout);
builder.Services.AddSingleton(channel);
*/
builder.Services.AddMassTransit(options => {
    options.UsingRabbitMq((context, cfg) => {
        cfg.Host(new Uri("rabbitmq://localhost:5672"), h => {
            h.Username("guest");
            h.Password("guest");
        });

    });
});

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


builder.Services.AddAuthorization();

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
