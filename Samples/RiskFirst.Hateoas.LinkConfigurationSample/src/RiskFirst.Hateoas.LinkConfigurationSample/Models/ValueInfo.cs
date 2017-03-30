using RiskFirst.Hateoas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.LinkConfigurationSample.Models
{
    public class ValueInfo : LinkContainer
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
