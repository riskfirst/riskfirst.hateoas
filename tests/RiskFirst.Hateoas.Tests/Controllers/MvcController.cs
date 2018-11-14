using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas.Models;
using RiskFirst.Hateoas.Tests.Controllers.Models;

namespace RiskFirst.Hateoas.Tests.Controllers
{
    [Route("api/[controller]")]
    public class MvcController : Controller
    {
        public const string GetAllValuesRoute = "GetAllValuesRoute";

        // GET api/values
        [HttpGet(Name = "GetAllValuesRoute")]
        public Task<ItemsLinkContainer<ValueInfo>> Get()
        {
            return Task.FromResult(new ItemsLinkContainer<ValueInfo>());
        }
    }
}
