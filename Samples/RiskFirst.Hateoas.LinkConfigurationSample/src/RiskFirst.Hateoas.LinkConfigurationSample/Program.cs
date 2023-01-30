using RiskFirst.Hateoas;
using RiskFirst.Hateoas.LinkConfigurationSample.Extensions;
using RiskFirst.Hateoas.LinkConfigurationSample.Models;
using RiskFirst.Hateoas.LinkConfigurationSample.Repository;
using RiskFirst.Hateoas.Models;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        services.AddControllers()
            .AddNewtonsoftJson();
        services.AddScoped<IValuesRepository, ValuesRepository>();
        services.AddLinks(config =>
        {
            // Uncomment out one of the two lines below to see how to configure the rel value of links to contain a uri to the returned model
            // 
            // config.ConfigureRelTransformation(transform => transform.AddProtocol().AddHost().AddModelPath());
            config.UseRelTransformation<ModelRelTransformation>();

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
                        .RequireRoutedLink("insert", "InsertValueRoute");
            });
        });

        var app = builder.Build();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}