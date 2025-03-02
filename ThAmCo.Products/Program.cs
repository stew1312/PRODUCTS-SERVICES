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
using ThAmCo.Products.Services.UnderCutters;

var builder = WebApplication.CreateBuilder(args);

// Loads the  Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Register the  Required Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the  Authentication with Debug Logging
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
                Console.WriteLine("Token successfully validated.");
                return Task.CompletedTask;
            }
        };
    });

// Do global authorisation 
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Configure Database Context
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

// Configure the Polly Retry Policy
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

// Register the Product Repository
builder.Services.AddTransient<IProductsRepo, ProductsRepo>();

var app = builder.Build();

// Seed Database in the Development Mode
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var env = services.GetRequiredService<IWebHostEnvironment>();
    
    if (env.IsDevelopment())
    {
        var context = services.GetRequiredService<ProductsContext>();

        try
        {
            // Count the number of products before seeding
            int productCount = context.Products.Count();
            Console.WriteLine($"📌 Current Product Count in DB: {productCount}");

            ProductsInitialiser.SeedTestData(context).Wait();

            // Count the number of products after seeding
            int newProductCount = context.Products.Count();
            Console.WriteLine($"✅ Updated Product Count in DB: {newProductCount}");
        }
        catch (Exception e)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError($"❌ Seeding test data failed: {e.Message}");
        }
    }
}

// Configure the Middleware
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
