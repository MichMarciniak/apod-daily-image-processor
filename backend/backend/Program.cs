using backend.Configuration;
using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var apiConfigSection = builder.Configuration.GetSection("Config");
var secretsConfigSection = builder.Configuration.GetSection("Secrets");

builder.Services.Configure<ApiConfig>(apiConfigSection);
builder.Services.Configure<SecretsConfig>(secretsConfigSection);

var apiConfig = apiConfigSection.Get<ApiConfig>();

if (apiConfig != null)
{
    builder.Services.AddHttpClient(apiConfig.ClientName, client =>
    {
        client.BaseAddress = new Uri(apiConfig.BaseApi);
    });
}

builder.Services.AddScoped<ConceptService>();
builder.Services.AddScoped<ImageService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();