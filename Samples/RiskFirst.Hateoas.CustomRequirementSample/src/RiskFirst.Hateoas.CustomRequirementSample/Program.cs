using RiskFirst.Hateoas;
using RiskFirst.Hateoas.CustomRequirementSample;
using RiskFirst.Hateoas.CustomRequirementSample.Models;
using RiskFirst.Hateoas.CustomRequirementSample.Repository;
using RiskFirst.Hateoas.CustomRequirementSample.Requirement;
using RiskFirst.Hateoas.Models;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        services.AddControllers()
            .AddNewtonsoftJson();

        services.AddTransient<ILinksHandler, ApiRootLinkHandler>();
        services.AddScoped<IValuesRepository, ValuesRepository>();

        services.AddLinks(config =>
        {
            config.AddPolicy<ValueInfo>(policy =>
            {
                policy.RequireRoutedLink("self", "GetValueByIdRoute", x => new { id = x.Id });
            });
            config.AddPolicy<ValueInfo>("FullInfoPolicy", policy =>
            {
                policy.RequireSelfLink()
                        .RequireRoutedLink("update", "GetValueByIdRoute", x => new { id = x.Id })
                        .RequireRoutedLink("delete", "DeleteValueRoute", x => new { id = x.Id })
                        .RequireRoutedLink("all", "GetAllValuesRoute");
            });

            config.AddPolicy<ItemsLinkContainer<ValueInfo>>(policy =>
            {
                policy.RequireSelfLink()
                        .RequiresApiRootLink() // Link to the API root from a collection of values
                        .RequireRoutedLink("insert", "InsertValueRoute");
            });

            config.AddPolicy<ApiInfo>(policy =>
            {
                policy.RequireSelfLink()
                        .RequireRoutedLink("values", "GetAllValuesRoute");
            });
        });

        var app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}