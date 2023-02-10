using RiskFirst.Hateoas.Models;

namespace RiskFirst.Hateoas.CustomRequirementSample.Models
{
    public class ApiInfo : LinkContainer
    {
        public string Version { get; set; }
    }
}