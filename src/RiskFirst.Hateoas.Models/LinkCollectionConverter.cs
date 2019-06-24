using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RiskFirst.Hateoas.Models
{
    public class LinkCollectionConverter : JsonConverter<LinkCollection>
    {
        public override LinkCollection ReadJson(JsonReader reader, Type objectType, LinkCollection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var links = (Dictionary<string, Link>)serializer
                .Deserialize(reader, typeof(Dictionary<string, Link>));

            foreach (var link in links)
            {
                link.Value.Name = link.Key;
                existingValue.Add(link.Value);
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, LinkCollection value, JsonSerializer serializer)
        {
            var links = value?
                .ToDictionary(x => x.Name, x => x);

            serializer.Serialize(writer, links);
        }
    }
}
