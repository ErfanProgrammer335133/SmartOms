using API.Middlewares;
using Application.Interfaces;
using Application.Services;
using Application.Utilities;
using Domain.Entities;
using Domain.IRepositories;
using Domain.Repositories;
using Infrastructure.Database_Context;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Database and DbContext
string local = builder.Configuration.GetConnectionString("local") ?? "";
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(local));
#endregion Database and DbContext

#region Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBusinessserviceService, BusinessserviceService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IJwtservice, JwtService>();
builder.Services.AddScoped<IPaymentService , PaymentService>();
builder.Services.AddScoped<IWalletService , WalletService>();
builder.Services.AddScoped<IUserService , UserService>();
builder.Services.AddScoped<IPasswordHasher<object> , PasswordHasher<object>>();
# endregion Services

#region Repositories
builder.Services.AddScoped<IBusinessServiceRepository , BusinessServiceRepository>();
builder.Services.AddScoped<ICartRepository , CartRepository>();
builder.Services.AddScoped<ICustomerRepository , CustomerRepository>();
builder.Services.AddScoped<IGenericRepository<object> , GenericRepository<object>>();
builder.Services.AddScoped<IInvoiceRepository , InvoiceRepository>();
builder.Services.AddScoped<IOrderRepository , OrderRepository>();
builder.Services.AddScoped<IPaymentTransactionRepository , PaymentTransactionRepository>();
builder.Services.AddScoped<IUserRepository , UserRepository>();
builder.Services.AddScoped<IWalletRepository , WalletRepository>();
builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
#endregion Repositories 

#region Swagger Barear
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
#endregion

#region Jwt Auth
byte[] key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "");

builder.Services.AddAuthentication(x =>
{
    x.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuerSigningKey = true ,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateIssuer= true ,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateAudience = true,
            ValidateLifetime = true 
        };
    });
#endregion Jwt Auth

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();

app.UseMiddleware<CustomAuthorizationMiddleware>();
app.UseMiddleware<CustomAuthenticationMiddleware>();
app.UseMiddleware<UserActiveMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
