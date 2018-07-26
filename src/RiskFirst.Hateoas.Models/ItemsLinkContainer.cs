using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.Models
{
    public class ItemsLinkContainer<T> : LinkContainer
    {
        private List<T> _items;
        public List<T> Items
        {
            get { return _items ?? (_items = new List<T>()); }
            set { _items = value; }
        }
    }
}
