using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace RiskFirst.Hateoas.Tests.Infrastructure
{
    public class AutonamedFactAttribute : FactAttribute
    {
        public AutonamedFactAttribute(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName]string testMethodName = "")
        {
            if(charsToReplace != null)
            {
                base.DisplayName = testMethodName?.Replace(charsToReplace, replacementChars);
            }
        }
    }

    [XunitTestCaseDiscoverer("Xunit.Sdk.TheoryDiscoverer", "xunit.execution.{Platform}")]
    public class AutonamedTheoryAttribute : AutonamedFactAttribute
    {
        public AutonamedTheoryAttribute(string charsToReplace = "_", string replacementChars = " ", [CallerMemberName]string testMethodName = "")
            : base(charsToReplace, replacementChars, testMethodName)
        { }
    }
}
