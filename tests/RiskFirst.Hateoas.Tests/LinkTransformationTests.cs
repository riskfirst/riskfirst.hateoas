using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RiskFirst.Hateoas.Tests
{
    public class LinkTransformationTests : IClassFixture<LinkTransformationTestContext>
    {
        private readonly LinkTransformationTestContext context;
        public LinkTransformationTests(LinkTransformationTestContext context)
        {
            this.context = context;
        }

        [Fact]
        public void GivenTransformations_DefaultLinksEvaluator_TransformsLinkSpec()
        {
            // Arrange
            var mockHrefTransform = new Mock<ILinkTransformation>();
            mockHrefTransform.Setup(x => x.Transform(It.IsAny<LinkTransformationContext>())).Returns("href");            
            var mockRelTransform = new Mock<ILinkTransformation>();
            mockRelTransform.Setup(x => x.Transform(It.IsAny<LinkTransformationContext>())).Returns("rel");
            var mockLinkSpec = new Mock<ILinkSpec>();
            mockLinkSpec.SetupGet(x => x.Id).Returns("testLink");
            mockLinkSpec.SetupGet(x => x.HttpMethod).Returns(HttpMethod.Get);

            context.Options.UseHrefTransformation(mockHrefTransform.Object);
            context.Options.UseRelTransformation(mockRelTransform.Object);

            // Act                        
            var underTest = new DefaultLinksEvaluator(Options.Create(context.Options), context);
            var model = new TestLinkContainer();
            underTest.BuildLinks(new[] { mockLinkSpec.Object }, model);

            // Assert
            Assert.True(model.Links.Count == 1, "Incorrect number of links applied");
            Assert.Equal("href", model.Links["testLink"].Href);
            Assert.Equal("rel", model.Links["testLink"].Rel);
            mockHrefTransform.Verify(x => x.Transform(It.IsAny<LinkTransformationContext>()), Times.Once());
            mockRelTransform.Verify(x => x.Transform(It.IsAny<LinkTransformationContext>()), Times.Once());
            mockLinkSpec.VerifyGet(x => x.HttpMethod);
            mockLinkSpec.VerifyGet(x => x.Id);
            
        }

        [Fact]
        public void GivenFormatters_BuilderLinkTransformation_GivesCorrectReslt()
        {
            // Arrange
            var list = new List<Func<LinkTransformationContext, string>>();
            list.Add(ctx => "a");
            list.Add(ctx => "b");
            list.Add(ctx => "c");
            var transformer = new BuilderLinkTransformation(list);
            var mockLinkSpec = new Mock<ILinkSpec>();
            var transformationContext = context.CreateContext(mockLinkSpec.Object);

            // Act
            var result = transformer.Transform(transformationContext);

            //Assert
            Assert.Equal("abc", result);
        }
    }
}
