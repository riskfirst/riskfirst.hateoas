using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RiskFirst.Hateoas.LinkConfigurationSample.Extensions
{
    public class ModelRelTransformation : ILinkTransformation
    {
        public string Transform(LinkTransformationContext context)
        {
            var sb = new StringBuilder();
            sb.Append(context.HttpContext.Request.Scheme)
                .Append("://")
                .Append(context.HttpContext.Request.Host.ToUriComponent())
                .Append("/models/");

            var returnType = context?.LinkSpec.RouteInfo?.MethodInfo?.ReturnType;
            var returnTypeInfo = returnType.GetTypeInfo();
            if (returnTypeInfo.IsGenericType)
            {
                if (returnType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)))
                {
                    sb.Append( GetTypePathInfo(returnType.GetGenericArguments()[0]));
                }
                else
                {
                    sb.Append(GetTypePathInfo(returnType));
                }
            }
            else
            {
                sb.Append(GetTypePathInfo(returnType));
            }
            return sb.ToString();
        }

        private static string GetTypePathInfo(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                var genericParam = type.GetGenericArguments()[0];
                return $"{Urlify(type.Namespace)}-{type.Name}/of/{GetTypePathInfo(genericParam)}";
            }
            else
            {
                return $"{Urlify(type.Namespace)}-{type.Name}";
            }
        }

        private static string Urlify(string str)
        {
            return str.Replace(".", "-");
        }
    }
}
