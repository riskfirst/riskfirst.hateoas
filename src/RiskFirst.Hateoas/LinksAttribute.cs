using System;

namespace RiskFirst.Hateoas
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class LinksAttribute : Attribute
    {
        public string Policy { get; set; }

        public LinksAttribute()
        {
        }
    }
}
