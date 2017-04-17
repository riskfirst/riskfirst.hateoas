using System;
using System.Collections.Generic;
using System.Text;

namespace RiskFirst.Hateoas
{
    public class LinkTransformationException : Exception
    {
        public LinkTransformationContext Context { get; }
        public LinkTransformationException(string message, LinkTransformationContext context)
            : base(message)
        {
            this.Context = context;
        }

        public LinkTransformationException(string message, Exception innerException, LinkTransformationContext context)
            : base(message,innerException)
        {
            this.Context = context;
        }
    }
}
