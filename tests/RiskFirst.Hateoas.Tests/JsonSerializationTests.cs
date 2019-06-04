using Newtonsoft.Json;
using RiskFirst.Hateoas.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RiskFirst.Hateoas.Tests
{
    public class JsonSerializationTests
    {
        [Fact]
        public void WhenLinksAreAvailable_ShouldSerializeThemProperly()
        {
            // Arrange
            var testLinks = new List<Link>
            {
                new Link { Name = "self", Href = "https://myamazingapp.com/12", Method = "GET", Rel = "self-rel" },
                new Link { Name = "create", Href = "https://myamazingapp.com/create", Method = "POST", Rel = "create-rel" },
                new Link { Name = "delete", Href = "https://myamazingapp.com/delete", Method = "DELETE", Rel = "delete-rel" }
            };

            var container = new TestLinkContainer(testLinks);

            // Act
            var result = JsonConvert.SerializeObject(container);

            // Assert
            var deserialized = JsonConvert.DeserializeObject<TestLinkContainer>(result);

            Assert.True(deserialized.Links.Count > 0);

            var linksAreCorrect = deserialized.Links.All(l => testLinks
                .Any(tl => tl.Href == l.Href &&
                           tl.Method == l.Method &&
                           tl.Name == l.Name &&
                           tl.Rel == l.Rel));

            Assert.True(linksAreCorrect);
        }

        [Fact]
        public void WhenNoLinksAreAvailable_NoLinksShouldBeSerialized()
        {
            // Arrange
            var testLinks = Enumerable.Empty<Link>();

            var container = new TestLinkContainer(testLinks);

            // Act
            var result = JsonConvert.SerializeObject(container);

            // Assert
            var deserialized = JsonConvert.DeserializeObject<TestLinkContainer>(result);

            Assert.Equal(0, deserialized.Links.Count);
        }
    }
}
