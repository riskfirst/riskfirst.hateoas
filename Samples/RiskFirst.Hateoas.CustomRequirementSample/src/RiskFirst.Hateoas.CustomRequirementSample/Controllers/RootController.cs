using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas.CustomRequirementSample.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RiskFirst.Hateoas.CustomRequirementSample.Controllers
{
    [Route("api")]
    public class RootController : Controller
    {
        private readonly ILinksService linksService;

        public RootController(ILinksService linksService)
        {
            this.linksService = linksService;
        }


        [HttpGet("",Name = "ApiRoot")]
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
