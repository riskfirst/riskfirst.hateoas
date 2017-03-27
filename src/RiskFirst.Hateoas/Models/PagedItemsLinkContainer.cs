using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Models
{
    public class PagedItemsLinkContainer<T> : ItemsLinkContainer<T>, IPagedLinkContainer
    {
        public int PageSize { get; set; }
        public int PageCount { get; set; }

        public int PageNumber { get; set; }
    }
}
