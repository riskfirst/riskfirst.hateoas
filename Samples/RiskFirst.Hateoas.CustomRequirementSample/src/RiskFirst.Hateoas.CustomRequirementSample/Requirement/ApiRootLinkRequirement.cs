namespace RiskFirst.Hateoas.CustomRequirementSample.Requirement
{
    public class ApiRootLinkRequirement : ILinksRequirement
    {
        public ApiRootLinkRequirement()
        {
        }
        public string Id { get; set; } = "root";
    }
}