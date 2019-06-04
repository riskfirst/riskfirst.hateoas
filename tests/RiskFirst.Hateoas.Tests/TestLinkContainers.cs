using RiskFirst.Hateoas.Models;
using System.Collections.Generic;

namespace RiskFirst.Hateoas.Tests
{
    public class DerivedLinkContainer : TestLinkContainer { }

    [Links()]
    public class EmptyOverrideTestLinkContainer : LinkContainer
    {
        public EmptyOverrideTestLinkContainer()
        {
        }

        public int Id { get; set; }
    }

    [Links(Policy = "OverridePolicy")]
    public class OverrideTestLinkContainer : LinkContainer
    {
        public OverrideTestLinkContainer()
        {
        }

        public int Id { get; set; }
    }

    public class TestLinkContainer : LinkContainer
    {
        public TestLinkContainer()
        {
        }

        public TestLinkContainer(IEnumerable<Link> links)
        {
            foreach (var link in links)
            {
                Links.Add(link);
            }
        }

        public int Id { get; set; }
    }

    public class TestPagedLinkContainer : PagedItemsLinkContainer<TestLinkContainer>
    {
    }

    public class UnrelatedLinkContainer : LinkContainer { }
}