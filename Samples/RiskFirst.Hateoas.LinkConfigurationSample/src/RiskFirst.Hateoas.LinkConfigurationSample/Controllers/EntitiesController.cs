using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using RiskFirst.Hateoas.Models;
using RiskFirst.Hateoas.LinkConfigurationSample.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace RiskFirst.Hateoas.LinkConfigurationSample.Controllers
{
    [Route("models")]
    public class EntitiesController : Controller
    {
        [HttpGet("{model}")]
        public object GetEntity(string model)
        {
            var type = model.Replace("-",".");
            return type;
        }
    }
}
