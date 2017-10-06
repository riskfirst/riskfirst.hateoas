using RiskFirst.Hateoas.Models;

namespace RiskFirst.Hateoas.Tests.Controllers.Models
{
    public class ValueInfo : LinkContainer
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
