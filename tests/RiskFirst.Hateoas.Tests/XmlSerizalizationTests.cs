using RiskFirst.Hateoas.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Xunit;

namespace RiskFirst.Hateoas.Tests
{
    public class XmlSerizalizationTests
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
            var xml = SerializeToXml(container);

            // Assert
            var result = DeserializeXml<TestLinkContainer>(xml);

            Assert.Equal(result.Links.Count, testLinks.Count);

            var linksAreCorrect = result.Links.All(l => testLinks
                .Any(tl => tl.Href == l.Href &&
                           tl.Method == l.Method &&
                           // We purposefully skip comparing by Rel because we expect it to be lost
                           // during the serialization/deserialization (and that's ok)
                           tl.Name == l.Name));

            Assert.True(linksAreCorrect);
        }

        [Fact]
        public void WhenNoLinksAreAvailable_NoLinksShouldBeSerialized()
        {
            // Arrange
            var testLinks = Enumerable.Empty<Link>();

            var container = new TestLinkContainer(testLinks);

            // Act
            var xml = SerializeToXml(container);

            // Assert
            var result = DeserializeXml<TestLinkContainer>(xml);

            Assert.Equal(0, result.Links.Count);
        }

        private static T DeserializeXml<T>(string xml)
        {
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        private static string SerializeToXml(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }
    }
}