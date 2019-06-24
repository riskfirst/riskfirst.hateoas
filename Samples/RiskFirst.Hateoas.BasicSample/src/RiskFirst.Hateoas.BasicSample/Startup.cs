using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiskFirst.Hateoas.BasicSample.Models;
using RiskFirst.Hateoas.BasicSample.Repository;
using RiskFirst.Hateoas.Models;
using System.Net.Http.Headers;

namespace RiskFirst.Hateoas.BasicSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLinks(config =>
            {
                // Uncomment the next line to use relative hrefs instead of absolute
                //config.UseRelativeHrefs();

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

            // Add framework services.
            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml").MediaType);
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json").MediaType);
            });

            services.AddSingleton<IValuesRepository, ValuesRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }
    }
}
