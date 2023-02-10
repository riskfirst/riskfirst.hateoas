using System.Diagnostics.CodeAnalysis;
using RiskFirst.Hateoas.Models;

namespace RiskFirst.Hateoas.CustomRequirementsSample.Tests
{
    public class LinkEqualityComparer : IEqualityComparer<Link>
    {
        public bool Equals(Link? x, Link? y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else if (x.Href == y.Href
                        && x.Method == y.Method
                        && x.Name == y.Name
                        && x.Rel == y.Rel)
                return true;
            else
                return false;
        }

        public int GetHashCode([DisallowNull] Link obj)
        {
            string hCode = $"{obj.Href}{obj.Method}{obj.Name}{obj.Rel}";
            return hCode.GetHashCode();
        }
    }
}