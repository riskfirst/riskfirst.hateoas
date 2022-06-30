using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace RiskFirst.Hateoas.Polyfills
{
    /// <inheritdoc />
    /// <summary>
    /// Default implementation for .Net Standard
    /// </summary>
    /// <seealso cref="T:RiskFirst.Hateoas.Polyfills.IAssemblyLoader" />
    internal class DefaultAssemblyLoader : IAssemblyLoader
    {
        public IEnumerable<Assembly> GetAssemblies()
        {
            var thisAssembly = GetType().GetTypeInfo().Assembly.GetName().Name;
            var libraries =
                DependencyContext.Default
                    .RuntimeLibraries
                    .Where(l => l.Dependencies.Any(d => d.Name.Equals(thisAssembly)));

            var names = libraries.Select(l => l.Name).Distinct();
            var assemblies = names.Select(a => Assembly.Load(new AssemblyName(a)));
            return assemblies;
        }
    }
}
