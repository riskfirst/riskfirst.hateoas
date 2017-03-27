namespace RiskFirst.Hateoas
{
    public interface ILinksRequirement
    {
    }

    public interface ILinksRequirement<TResource> : ILinksRequirement
    {
        ILinksRequirement<T> Convert<T>() where T : class;
    }
}
