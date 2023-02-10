using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas.CustomRequirementSample.Models;

namespace RiskFirst.Hateoas.CustomRequirementSample.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        private readonly ILinksService linksService;

        public RootController(ILinksService linksService)
        {
            this.linksService = linksService;
        }


        [HttpGet("", Name = "ApiRoot")]
        public async Task<ApiInfo> GetApiInfo()
        {
            var info = new ApiInfo()
            {
                Version = "v1.0"
            };
            await linksService.AddLinksAsync(info);

            return info;
        }
    }
}