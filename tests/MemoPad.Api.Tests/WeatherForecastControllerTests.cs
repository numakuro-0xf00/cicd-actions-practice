using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MemoPad.Api.Tests;

public class WeatherForecastControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Get_ReturnsOk()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/WeatherForecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
