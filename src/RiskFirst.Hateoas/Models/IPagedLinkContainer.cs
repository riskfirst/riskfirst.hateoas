using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Models
{
    public interface IPagedLinkContainer : ILinkContainer
    {
        int PageSize { get; set; }
        int PageNumber { get; set; }
        int PageCount { get; set; }
    }
}
