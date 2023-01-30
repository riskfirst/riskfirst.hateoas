using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RiskFirst.Hateoas.CustomRequirementSample.Models;
using RiskFirst.Hateoas.CustomRequirementsSample.Tests;
using RiskFirst.Hateoas.Models;

namespace RiskFirst.Hateoas.CustomRequirementsSample.TestsNew;

public class RootApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RootApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllValues_Json_ReturnsLinksWithRootApi()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/values");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var values = JsonConvert.DeserializeObject<ItemsLinkContainer<ValueInfo>>(responseString);

        Assert.Contains(
            new Link
            {
                Href = "http://localhost/api",
                Method = "GET",
                Name = "root",
                Rel = "Root/ApiRoot"
            },
            values.Links,
            new LinkEqualityComparer());
    }
}
