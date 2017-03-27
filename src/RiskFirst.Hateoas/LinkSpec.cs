using System.Net.Http;

namespace RiskFirst.Hateoas
{
    public class LinkSpec
    {
        public string Id { get; set; }
        public string RouteName { get; set; }
        public string ControllerName { get; set; }
        public object Values { get; set; }
        public HttpMethod Method { get; set; }
    }
}
