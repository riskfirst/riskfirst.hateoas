using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas.Models;
using RiskFirst.Hateoas.Tests.Controllers.Models;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Tests.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        public const string GetAllValuesRoute = "GetAllValuesApiRoute";

        // GET api/values
        [HttpGet(Name = "GetAllValuesApiRoute")]
        public Task<ItemsLinkContainer<ValueInfo>> Get()
        {
            return Task.FromResult(new ItemsLinkContainer<ValueInfo>());
        }
    }
}
