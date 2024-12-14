using Crypto_Portfolio_API.Data;
using Crypto_Portfolio_API.Exceptions;
using Crypto_Portfolio_API.Market.repository;
using Crypto_Portfolio_API.Market.repository.impl;
using Crypto_Portfolio_API.Market.service;
using Crypto_Portfolio_API.Market.service.impl;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//REGISTER REPOSITORY
builder.Services.AddScoped<ICryptoPriceRepository, CryptoPriceRepository>();

//REGISTER SERVICE
builder.Services.AddScoped<ICryptoPriceService, CryptoPriceService>();

builder.Services.AddHttpClient();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoPortfolioAPI", Version = "V1" });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
ApplyDataBaseSeed();
void ApplyDataBaseSeed()
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (db.Database.GetPendingMigrations().Any())
        {
            db.Database.Migrate();
        }
    }
}
