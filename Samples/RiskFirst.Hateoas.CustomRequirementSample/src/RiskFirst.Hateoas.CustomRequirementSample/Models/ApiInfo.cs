using RiskFirst.Hateoas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.CustomRequirementSample.Models
{
    public class ApiInfo : LinkContainer
    {
        public string Version { get; set; }
    }
}
