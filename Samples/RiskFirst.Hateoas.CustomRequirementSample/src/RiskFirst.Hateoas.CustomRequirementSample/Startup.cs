using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiskFirst.Hateoas.CustomRequirementSample.Models;
using RiskFirst.Hateoas.Models;
using RiskFirst.Hateoas.CustomRequirementSample.Repository;
using System.Reflection;
using RiskFirst.Hateoas.CustomRequirementSample.Requirement;

namespace RiskFirst.Hateoas.CustomRequirementSample
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
                config.AddPolicy<ValueInfo>(policy =>
                {
                    policy.RequireRoutedLink("self", "GetValueByIdRoute", x => new { id = x.Id });
                });
                config.AddPolicy<ValueInfo>("FullInfoPolicy",policy =>
                {
                    policy.RequireSelfLink()
                            .RequireRoutedLink("update", "GetValueByIdRoute", x => new { id = x.Id })
                            .RequireRoutedLink("delete", "DeleteValueRoute", x => new { id = x.Id })
                            .RequireRoutedLink("all", "GetAllValuesRoute");
                });

                config.AddPolicy<ItemsLinkContainer<ValueInfo>>(policy =>
                {
                    policy.RequireSelfLink()
                            .RequiresApiRootLink()
                            .RequireRoutedLink("insert", "InsertValueRoute");
                });

                config.AddPolicy<ApiInfo>(policy =>
                {
                    policy.RequireSelfLink()
                            .RequireRoutedLink("values", "GetAllValuesRoute");
                });
            });

            // Add framework services.
            services.AddMvc();

            services.AddTransient<ILinksHandler, RootLinkHandler>();
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
