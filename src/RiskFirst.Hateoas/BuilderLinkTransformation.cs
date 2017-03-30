using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class BuilderLinkTransformation : ILinkTransformation
    {
        private readonly IList<Func<LinkTransformationContext, string>> transforms;
        public BuilderLinkTransformation(IList<Func<LinkTransformationContext,string>> transforms)
        {
            this.transforms = transforms;
        }

        public string Transform(LinkTransformationContext context)
        {
            var builder = transforms.Aggregate(new StringBuilder(), (sb, transform) =>
            {
                sb.Append(transform(context));
                return sb;
            });
            return builder.ToString();
        }
    }
}
