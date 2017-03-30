using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class LinkTransformationBuilder
    {
        private IList<Func<LinkTransformationContext, string>> Transformations { get; } = new List<Func<LinkTransformationContext, string>>();        
       
        public LinkTransformationBuilder Add(string value)
        {
            Transformations.Add(ctx => value);
            return this;
        }
        public LinkTransformationBuilder Add(Func<LinkTransformationContext,string> getValue)
        {
            Transformations.Add(getValue);
            return this;
        }

        public ILinkTransformation Build()
        {
            return new BuilderLinkTransformation(Transformations);
        }
        
       
    }
}
