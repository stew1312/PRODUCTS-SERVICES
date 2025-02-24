using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using ThAmCo.Products.Data.Products;
using ThAmCo.Products.Services.ProductsRepo;
using ThAmCo.Products.Services.UnderCutters;
using Polly;
using Polly.Extensions.Http;
using ThAnCo.Products.Services.UnderCutters;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ✅ Register Required Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Configure Authentication with Debug Logging
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Domain"]?.TrimEnd('/');
        options.Audience = builder.Configuration["Auth:Audience"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"❌ Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("✅ Token successfully validated.");
                return Task.CompletedTask;
            }
        };
    });

// ✅ Enforce Authorization Globally
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ✅ Configure Database Context
builder.Services.AddDbContext<ProductsContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "products.db");
        options.UseSqlite($"Data Source={dbPath}");

        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
    else
    {
        var cs = builder.Configuration.GetConnectionString("ProductsContext");
        options.UseSqlServer(cs, sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(2),
                errorNumbersToAdd: null
            );
        });
    }
});

// ✅ Configure Polly Retry Policy
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IUnderCuttersService, UnderCuttersServiceFake>();
}
else
{
    builder.Services.AddHttpClient<IUnderCuttersService, UnderCuttersService>()
        .AddPolicyHandler(retryPolicy);
}

// ✅ Register Product Repository
builder.Services.AddTransient<IProductsRepo, ProductsRepo>();

var app = builder.Build();

// ✅ Seed Database in Development Mode
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var env = services.GetRequiredService<IWebHostEnvironment>();
    if (env.IsDevelopment())
    {
        var context = services.GetRequiredService<ProductsContext>();
        try
        {
            ProductsInitialiser.SeedTestData(context).Wait();
        }
        catch (Exception e)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogDebug($"Seeding test data failed: {e.Message}");
        }
    }
}

// ✅ Configure Middleware
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
