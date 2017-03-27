namespace RiskFirst.Hateoas
{
    public interface ILinkHelper
    {
        string GetHref(LinkSpec link);
        string GetRel(LinkSpec link);
    }
}
