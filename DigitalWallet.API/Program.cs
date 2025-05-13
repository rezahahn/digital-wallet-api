using DigitalWallet.Application.DTOs.User;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Application.Mappings;
using DigitalWallet.Application.Services;
using DigitalWallet.Core.Interfaces;
using DigitalWallet.Infrastructure.Common.Helpers;
using DigitalWallet.Infrastructure.Data;
using DigitalWallet.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using DigitalWallet.Application.DTOs.Wallet.Request.Validators;
using DigitalWallet.Infrastructure.Messaging;
using DigitalWallet.Application.DTOs.Messaging;
using DigitalWallet.Application.EventHandlers;
using DigitalWallet.Core.Interfaces.Messaging;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// ========== [1] Add Services to Container ========== //

// (A) Controller & API Behavior
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddApiVersioning(options =>
//{
//    options.DefaultApiVersion = new ApiVersion(1, 0);
//    options.AssumeDefaultVersionWhenUnspecified = true;
//    options.ReportApiVersions = true;
//    options.ApiVersionReader = new UrlSegmentApiVersionReader();
//});

//builder.Services.AddVersionedApiExplorer(options =>
//{
//    options.GroupNameFormat = "'v'VVV";
//    options.SubstituteApiVersionInUrl = true;
//});
builder.Services.AddValidatorsFromAssemblyContaining<TopUpRequestValidator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// (B) Database Context (SQL Server)
builder.Services.AddDbContext<DigitalWalletContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    ServiceLifetime.Scoped);

// (C) Register Repositories (Infrastructure Layer)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>(); ;

// (D) Register Services (Application Layer)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// (E) Helpers & Utilities
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddDistributedMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "DigitalWallet_";
});

// (F) AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// (G) FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();

// (H) RabbitMQ
builder.Services.Configure<RabbitMQConfig>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
builder.Services.AddScoped<WalletEventHandler>();

// Konfigurasi JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// ========== [2] Build & Middleware Setup ========== //
var app = builder.Build();

// Setup RabbitMQ Consumer
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    var eventBus = app.Services.GetRequiredService<IEventBus>();
    var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

    eventBus.Subscribe<CreateWalletMessage>(
    "create_wallet_queue.work",
    async message => {
        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<WalletEventHandler>();
            await handler.HandleCreateWalletMessage(message);
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error processing wallet creation message");
        }
    });
}

// (A) Development Config
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// (B) Production Config
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// (C) Run App
app.Run();