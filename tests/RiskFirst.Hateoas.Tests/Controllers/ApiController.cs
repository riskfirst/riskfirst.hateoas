using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas.Models;
using RiskFirst.Hateoas.Tests.Controllers.Models;

namespace RiskFirst.Hateoas.Tests.Controllers
{
    [Controller]
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
