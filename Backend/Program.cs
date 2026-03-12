using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagementSystem.Data;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Lab.Lab2.AbstractFactory;
using OrderManagementSystem.Lab.Lab2.AbstractFactory.PayPal;
using OrderManagementSystem.Lab.Lab2.AbstractFactory.Stripe;
using OrderManagementSystem.Lab.Lab3.Prototype;
using OrderManagementSystem.Lab.Lab3.Singleton;
using OrderManagementSystem.Lab.Lab4.Adapter;
using OrderManagementSystem.Lab.Lab4.Adapter.DPD;
using OrderManagementSystem.Lab.Lab4.Adapter.FanCourier;
using OrderManagementSystem.Lab.Lab4.Facade;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "http://localhost:3001",
                "http://127.0.0.1:3001")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<StripeProviderFactory>();
builder.Services.AddScoped<PayPalProviderFactory>();
builder.Services.AddScoped<IPaymentProviderFactory>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

    var requestedProvider = contextAccessor.HttpContext?.Request.Headers["X-Payment-Provider"].ToString();
    var configuredProvider = configuration["PaymentProvider"] ?? "Stripe";
    var activeProvider = string.IsNullOrWhiteSpace(requestedProvider) ? configuredProvider : requestedProvider;

    return activeProvider.Equals("PayPal", StringComparison.OrdinalIgnoreCase)
        ? serviceProvider.GetRequiredService<PayPalProviderFactory>()
        : serviceProvider.GetRequiredService<StripeProviderFactory>();
});
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<OrderTemplateService>();

// Lab4 — Adapter (Shipping)
builder.Services.AddSingleton<FanCourierClient>();
builder.Services.AddSingleton<DpdApiClient>();
builder.Services.AddScoped<IShippingProvider, FanCourierAdapter>();
builder.Services.AddScoped<IShippingProvider, DpdAdapter>();
builder.Services.AddScoped<ShippingService>();

// Lab4 — Facade (Checkout)
builder.Services.AddScoped<IEmailNotificationService, ConsoleEmailNotificationService>();
builder.Services.AddScoped<OrderPlacementFacade>();

var secret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(secret))
{
    throw new InvalidOperationException("JWT secret is missing in appsettings.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token only. Example: eyJhbGciOi..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
