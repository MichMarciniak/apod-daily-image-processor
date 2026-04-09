using backend.Mappers;
using backend.Models;
using FluentAssertions;
using Microsoft.JSInterop.Infrastructure;

namespace ApodProcessor.Tests;

public class MappingTests
{
    [Fact]
    public void Image_ToDto_ShouldMapAllFieldsCorrectly()
    {
        var image = new Image
        {
            Title = "Moon",
            Explanation = "This is moon",
            Copyright = "none",
            HdPath = "https://nasa.gov/moon.jpg",
            Date = DateTime.Parse("2026-04-01")
        };

        var dto = image.ToDto();

        dto.Should().NotBeNull();
        dto.Title.Should().Be(image.Title);
        dto.Explanation.Should().Be(image.Explanation);
        dto.Copyright.Should().Be(image.Copyright);
        dto.HdPath.Should().Be(image.HdPath);
        dto.Date.Should().Be(image.Date);
    }
}