using System.Net.Http.Headers;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using RiskFirst.Hateoas.BasicSample.Models;
using RiskFirst.Hateoas.Models;

namespace RiskFirst.Hateoas.BasicSample.Tests;

public class BasicTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BasicTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllValues_Json_ReturnsObjectsWithLinks()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/values");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var values = JsonConvert.DeserializeObject<ItemsLinkContainer<ValueInfo>>(responseString);

        Assert.All(values.Items, i => Assert.True(i.Links.Count > 0, "Invalid number of links"));
    }

    [Fact]
    public async Task GetAllValues_Xml_ReturnsObjectsWithLinks()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        var response = await client.GetAsync("/api/values");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var values = DeserializeXml<ItemsLinkContainer<ValueInfo>>(responseString);

        Assert.All(values.Items, i => Assert.True(i.Links.Count > 0, "Invalid number of links"));
    }

    [Fact]
    public async Task GetValue_Json_AlternateRoute_ReturnsObjectsWithLinks()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/values/v2/1");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var value = JsonConvert.DeserializeObject<ValueInfo>(responseString);

        Assert.True(value.Links.Count > 0, "Invalid number of links");
    }

    [Fact]
    public async Task GetValue_Xml_AlternateRoute_ReturnsObjectsWithLinks()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        var response = await client.GetAsync("/api/values/v2/1");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var value = DeserializeXml<ValueInfo>(responseString);

        Assert.True(value.Links.Count > 0, "Invalid number of links");
    }

    [Fact]
    public async Task GetValue_Json_ReturnsObjectsWithLinks()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/values/1");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var value = JsonConvert.DeserializeObject<ValueInfo>(responseString);

        Assert.True(value.Links.Count > 0, "Invalid number of links");
    }

    [Fact]
    public async Task GetValue_Xml_ReturnsObjectsWithLinks()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        var response = await client.GetAsync("/api/values/1");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var value = DeserializeXml<ValueInfo>(responseString);

        Assert.True(value.Links.Count > 0, "Invalid number of links");
    }

    private static T DeserializeXml<T>(string xml)
    {
        using (var reader = new StringReader(xml))
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(reader);
        }
    }
}
