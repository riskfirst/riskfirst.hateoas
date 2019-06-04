using Newtonsoft.Json;
using System.Xml.Serialization;

namespace RiskFirst.Hateoas.Models
{
    public class Link
    {
        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("method")]
        public string Method { get; set; }

        [XmlAttribute("rel")]
        [JsonIgnore]
        public string Name { get; set; }

        [XmlIgnore]
        public string Rel { get; set; }
    }
}