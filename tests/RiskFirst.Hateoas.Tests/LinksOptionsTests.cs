using RiskFirst.Hateoas.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace RiskFirst.Hateoas.Tests
{
    public class LinksOptionsTests
    {
        private readonly LinksOptions linksOptions;
        
        public LinksOptionsTests()
        {
            linksOptions = new LinksOptions();
        }

        [AutonamedFact]
        public void WhenAddingABuiltPolicy_PolicyShouldBeMappedCorrectly()
        {
            // Arrange
            var policyToAdd = BuildTestPolicy();

            // Act
            linksOptions.AddPolicy<TestLinkContainer>(policyToAdd);

            // Assert
            var addedPolicy = linksOptions.GetPolicy<TestLinkContainer>();

            AssertPoliciesMatch(policyToAdd, addedPolicy);
        }

        [AutonamedFact]
        public void WhenAddingANamedBuiltPolicy_PolicyShouldBeMappedCorrectly()
        {
            // Arrange
            var policyName = "TestName";
            var policyToAdd = BuildTestPolicy();

            // Act
            linksOptions.AddPolicy<TestLinkContainer>(policyName, policyToAdd);

            // Assert
            var addedPolicy = linksOptions.GetPolicy<TestLinkContainer>(policyName);

            AssertPoliciesMatch(policyToAdd, addedPolicy);
        }

        [AutonamedFact]
        public void WhenAddingAPolicyWithNullName_ExpectArgumentException()
        {
            // Arrange
            var policy = BuildTestPolicy();

            // Act, Assert
            Assert.Throws<ArgumentException>(() =>
            {
                linksOptions.AddPolicy<TestLinkContainer>(null, policy);
            });
        }

        [AutonamedFact]
        public void WhenAddingANullPolicy_ExpectArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                linksOptions.AddPolicy<TestLinkContainer>((ILinksPolicy)null);
            });
        }

        [AutonamedFact]
        public void WhenAddingNullNamedPolicy_ExpectArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                linksOptions.AddPolicy<TestLinkContainer>("SomeName", (ILinksPolicy)null);
            });
        }

        private static void AssertPoliciesMatch(ILinksPolicy policyToAdd, ILinksPolicy addedPolicy)
        {
            Assert.NotNull(addedPolicy);
            Assert.True(policyToAdd.Requirements.Count == addedPolicy.Requirements.Count);

            var expectedRequirements = policyToAdd.Requirements;

            Assert.True(policyToAdd.Requirements.All(r => expectedRequirements.Any(er => er.GetType() == r.GetType())));
        }

        private static ILinksPolicy BuildTestPolicy() =>
            new LinksPolicyBuilder<TestLinkContainer>()
                .RequireSelfLink()
                .RequireRoutedLink("some-link", "ArbitraryRouteName")
                .Build();
    }
}
