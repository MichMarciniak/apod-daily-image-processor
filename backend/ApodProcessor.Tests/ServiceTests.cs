using System.Diagnostics.CodeAnalysis;
using backend.Configuration;
using backend.Data;
using backend.Dtos;
using backend.Models;
using backend.Services;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApodProcessor.Tests;

public class ServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ImageService _service;
    private readonly SqliteConnection _connection;
    private readonly string _testImageDir;

    public ServiceTests()
    {
        // --- DB ---
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;
        
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        // --- Example Image --- 
        _testImageDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var apiConfig = new ApiConfig { ImageDir = _testImageDir };
        var wrappedOptions = Options.Create(apiConfig);

        _service = new ImageService(_context, wrappedOptions);
    }

    public void Dispose()
    {
        _connection.Close();
        _context.Dispose();
        if (Directory.Exists(_testImageDir))
            Directory.Delete(_testImageDir, true);
    }

    [Fact]
    public async Task SaveImageFromApi_ShouldReturnNull_IfImageWithSameDateAlreadyExists()
    {
        var date = new DateTime(2026, 04, 09);
        var existingImage = new Image { Date = date, Title = "Exist" };
        _context.Images.Add(existingImage);
        await _context.SaveChangesAsync();

        var response = new ApodApiResponse { Date = date, Title = "New" };
        var stream = new MemoryStream();

        var result = await _service.SaveImageFromApi(response, stream, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveImageFromApi_ShouldCreateFileOnDisk()
    {
        var response = new ApodApiResponse
        {
            Date = DateTime.Now,
            Title = "Test",
            Explanation = "Testing disk"
        };

        var content = new byte[] { 1, 2, 3 };
        var stream = new MemoryStream(content);

        var id = await _service.SaveImageFromApi(response, stream, CancellationToken.None);
        id.Should().NotBeNull();
        var expedtedPath = Path.Combine(_testImageDir, "images", "hd", $"{response.Date:yyyy-MM-dd}.jpg");
        File.Exists(expedtedPath).Should().BeTrue();
    }
}