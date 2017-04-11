using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas
{
    public class ReflectionControllerMethodInfo : IControllerMethodInfo
    {
        private readonly MethodInfo methodInfo;
        public ReflectionControllerMethodInfo(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
        }
        public Type ControllerType => methodInfo.DeclaringType;

        public Type ReturnType => methodInfo.ReturnType;

        public string MethodName => methodInfo.Name;

        public IEnumerable<TAttribute> GetAttributes<TAttribute>() where TAttribute : Attribute
        {
            return methodInfo.GetCustomAttributes<TAttribute>()
                    .Union(ControllerType.GetTypeInfo().GetCustomAttributes<TAttribute>());
        }
        
    }
}
