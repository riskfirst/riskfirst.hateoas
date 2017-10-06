using System.Linq;
using System.Reflection;
using RiskFirst.Hateoas.Polyfills;
using RiskFirst.Hateoas.Tests.Infrastructure;
using Xunit;

namespace RiskFirst.Hateoas.Tests.Polyfills
{
    [Trait("Category", "Polyfills")]
    public class DefaultAssemblyLoaderTests
    {
        [AutonamedFact]
        public void WhenGetAssemblies_ShouldReturnAssemblies()
        {
            var loader = new DefaultAssemblyLoader();
            var assemblies = loader.GetAssemblies();

            assemblies.First(a => a.FullName == GetType().GetTypeInfo().Assembly.FullName);
        }
    }
}
