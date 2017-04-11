using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public interface IControllerMethodInfo
    {
        Type ControllerType { get; }
        Type ReturnType { get; }
        string MethodName { get; }
        IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute: Attribute;
    }
}
