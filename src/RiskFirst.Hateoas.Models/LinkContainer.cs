using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Xml.Serialization;

namespace RiskFirst.Hateoas.Models
{
    public abstract class LinkContainer : ILinkContainer
    {
        [XmlElement("link")]
        [JsonProperty(PropertyName = "_links")]
        public LinkCollection Links { get; set; } = new LinkCollection();

        public void Add(Link link)
        {
            Links.Add(link);
        }
    }
}
