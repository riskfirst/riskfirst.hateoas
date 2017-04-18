using RiskFirst.Hateoas.Models;

namespace RiskFirst.Hateoas.Tests
{
    public class TestLinkContainer : LinkContainer
    {
        public int Id { get; set; }
        public TestLinkContainer()
        {
        }
    }

    public class DerivedLinkContainer : TestLinkContainer { }

    public class UnrelatedLinkContainer : LinkContainer { }


    [Links(Policy = "OverridePolicy")]
    public class OverrideTestLinkContainer : LinkContainer
    {
        public int Id { get; set; }
        public OverrideTestLinkContainer()
        {
        }
    }

    [Links()]
    public class EmptyOverrideTestLinkContainer : LinkContainer
    {
        public int Id { get; set; }
        public EmptyOverrideTestLinkContainer()
        {
        }
    }

    public class TestPagedLinkContainer : PagedItemsLinkContainer<TestLinkContainer>
    {
    }
}