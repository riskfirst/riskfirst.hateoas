using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface ILinkTransformation
    {
        string Transform(LinkTransformationContext context);
    }
}
