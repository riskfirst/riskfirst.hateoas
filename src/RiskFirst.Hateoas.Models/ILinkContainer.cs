namespace RiskFirst.Hateoas.Models
{
    public interface ILinkContainer
    {
        LinkCollection Links { get; set; }
        void Add(Link link);
    }
}
