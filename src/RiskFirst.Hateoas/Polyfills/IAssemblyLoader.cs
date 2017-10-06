using System.Collections.Generic;
using System.Reflection;

namespace RiskFirst.Hateoas.Polyfills
{
    /// <summary>
    /// Abstracts AppDomain functionallity 
    /// </summary>
    public interface IAssemblyLoader
    {
        IEnumerable<Assembly> GetAssemblies();
    }
}
