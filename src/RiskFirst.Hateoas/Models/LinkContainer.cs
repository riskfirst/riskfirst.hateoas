using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace RiskFirst.Hateoas.Models
{
    public abstract class LinkContainer : ILinkContainer
    {
        private Dictionary<string, Link> links;
        [JsonProperty(PropertyName = "_links")]
        public Dictionary<string, Link> Links
        {
            get { return links ?? (links = new Dictionary<string, Link>()); }

            set { links = value; }
        }

        public void AddLink(string id, Link link)
        {
            Links.Add(id, link);
        }
    }
}
