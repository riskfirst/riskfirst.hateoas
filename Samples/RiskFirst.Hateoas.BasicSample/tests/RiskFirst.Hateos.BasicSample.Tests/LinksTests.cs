using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using RiskFirst.Hateoas.BasicSample;
using RiskFirst.Hateoas.BasicSample.Models;
using RiskFirst.Hateoas.Models;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace RiskFirst.Hateos.BasicSample.Tests
{
    public class LinksTests
    {
        private readonly HttpClient client;
        private readonly TestServer server;

        public LinksTests()
        {
            // Arrange
            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            client = server.CreateClient();
        }

        [Fact]
        public async Task GetAllValues_Json_ReturnsObjectsWithLinks()
        {
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
            // Act
            var response = await client.GetAsync("/api/values?format=xml");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var values = DeserializeXml<ItemsLinkContainer<ValueInfo>>(responseString);

            Assert.All(values.Items, i => Assert.True(i.Links.Count > 0, "Invalid number of links"));
        }

        [Fact]
        public async Task GetValue_Json_AlternateRoute_ReturnsObjectsWithLinks()
        {
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
            // Act
            var response = await client.GetAsync("/api/values/v2/1?format=xml");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var value = DeserializeXml<ValueInfo>(responseString);

            Assert.True(value.Links.Count > 0, "Invalid number of links");
        }

        [Fact]
        public async Task GetValue_Json_ReturnsObjectsWithLinks()
        {
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
            // Act
            var response = await client.GetAsync("/api/values/1?format=xml");
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
}