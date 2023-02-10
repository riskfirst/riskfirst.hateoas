using System.Reflection;

namespace RiskFirst.Hateoas.LinkConfigurationSample.Extensions
{
    public static class LinkTransformationBuilderExtensions
    {
        public static LinkTransformationBuilder AddModelPath(this LinkTransformationBuilder builder)
        {
            return builder.Add("/models/").Add(ctx =>
            {
                var returnType = ctx?.LinkSpec.RouteInfo?.MethodInfo?.ReturnType;
                var returnTypeInfo = returnType.GetTypeInfo();
                if (returnTypeInfo.IsGenericType)
                {
                    if (returnType.GetGenericTypeDefinition().IsAssignableFrom(typeof(Task<>)))
                    {
                        return GetTypePathInfo(returnType.GetGenericArguments()[0]);
                    }
                }
                return GetTypePathInfo(returnType);

            });
        }

        private static string GetTypePathInfo(Type type)
        {
            if (type.IsConstructedGenericType)
            {
                var genericParam = type.GetGenericArguments()[0];
                return $"{type.Namespace.Urlify()}-{type.Name}/of/{GetTypePathInfo(genericParam)}";
            }
            else
            {
                return $"{type.Namespace.Urlify()}-{type.Name}";
            }
        }

        private static string Urlify(this string str)
        {
            return str.Replace(".", "-");
        }
    }
}