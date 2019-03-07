using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using Xunit;
using RiskFirst.Hateoas.BasicSample;
using Newtonsoft.Json;
using RiskFirst.Hateoas.BasicSample.Models;
using System.Collections.Generic;

namespace RiskFirst.Hateos.BasicSample.Tests
{
    public class LinksTests
    {
        private readonly TestServer server;
        private readonly HttpClient client;
        public LinksTests()
        {
            // Arrange
            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            client = server.CreateClient();
        }
        [Fact]
        public async Task GetAllValues_ReturnsObjectsWithLinks()
        {
            // Act
            var response = await client.GetAsync("/api/values");
            response.EnsureSuccessStatusCode();
            
            var responseString = await response.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<ValuesContainer>(responseString);

            Assert.All(values.Items, i => Assert.True(i.Links.Count>0,"Invalid number of links"));
        }

        [Fact]
        public async Task GetValue_ReturnsObjectsWithLinks()
        {
            // Act
            var response = await client.GetAsync("/api/values/1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<ValueInfo>(responseString);

            Assert.True(value.Links.Count > 0, "Invalid number of links");
        }

        [Fact]
        public async Task GetValue_AlternateRoute_ReturnsObjectsWithLinks()
        {
            // Act
            var response = await client.GetAsync("/api/values/v2/1");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var value = JsonConvert.DeserializeObject<ValueInfo>(responseString);

            Assert.True(value.Links.Count > 0, "Invalid number of links");
        }
    }

    public class ValuesContainer
    {
        public IEnumerable<ValueInfo> Items { get; set; }
    }
}
