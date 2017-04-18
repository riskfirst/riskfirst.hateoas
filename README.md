# RiskFirst.Hateoas

![CI Build](https://riskfirst.visualstudio.com/_apis/public/build/definitions/2e2e46bb-1ab7-484e-8117-335c3855d65d/326/badge)

An implementation of [HATEOAS](https://en.wikipedia.org/wiki/HATEOAS) for aspnet core web api projects which gives full control of which links to apply to models returned from your api. In order to communicate varying state to the end-user, this library fully integrates with Authorization, and allows arbitrary conditions to determine whether to show or hide HATEOAS links between api resources.

### Getting started

Install the package from [Nuget.org](https://www.nuget.org/packages/riskfirst.hateoas)

```powershell
PM> Install-Package RiskFirst.Hateoas
```

Configure the links to include for each of your models.

```csharp
public class Startup
{
  public void ConfigureServices(IServicesCollection services)
  {
    services.AddLinks(config => 
    {
      config.AddPolicy<MyModel>(policy => {
          policy.RequiresSelfLink()
                .RequiresRoutedLink("all", "GetAllModelsRoute")
                .RequiresRoutedLink("delete", "DeleteModelRoute", x => new { id = x.Id });
      });
    });
  }
}
```

Inject ```ILinksService``` into any controller (or other class in your project) to add links to a model.

```csharp
[Route("api/[controller]")]
public class MyController : Controller
{
    private readonly ILinksService linksService;
    
    public MyController(ILinksService linksService)
    {
        this.linksService = linksService;
    }
   
    [HttpGet("{id}",Name = "GetModelRoute")]
    public async Task<MyModel> GetMyModel(int id)
    {
         var model = await myRepository.GetMyModel(id);
         await linksService.AddLinksAsync(model);
         return model;
    }
    [HttpGet(Name="GetAllModelsRoute")]
    public async Task<IEnumerable<MyModel>> GetAllModels()
    {
         //... snip .. //
    }
    
    [HttpDelete("{id}",Name = "DeleteModelRoute")]
    public async Task<MyModel> DeleteMyModel(int id)
    {
         //... snip .. //
    }
}
```

The above code would produce a response as the example below

```javascript
{
   id:1,
   someOtherField: "foo",
   _links : {
    self: {
      rel: "MyController\GetModelRoute",
      href: "https://api.example.com/my/1"
      method: "GET"
    },
    all: {
      rel: "MyController\GetAllModelsRoute",
      href: "https://api.example.com/my"
      method: "GET"
    },
    delete: {
      rel: "MyController\GetAllModelsRoute",
      href: "https://api.example.com/my/1"
      method: "DELETE"
    }
  }
}
```

### Multiple policies for a model

It is possible to specify multiple named policies for a model during startup by providing a policy name to ```AddPolicy```. For example, you could have the default (unnamed) policy give basic links when the model is part of a list, but more detailed information when a model is returned alone.

```csharp
public class Startup
{
  public void ConfigureServices(IServicesCollection services)
  {
    services.AddLinks(config => 
    {
      config.AddPolicy<MyModel>(policy => {
          policy.RequiresRoutedLink("self","GetModelRoute", x => new {id = x.Id })
      });
      
      config.AddPolicy<MyModel>("FullInfo",policy => {
          policy.RequiresSelfLink()
                .RequiresRoutedLink("all", "GetAllModelsRoute")
                .RequiresRoutedLink("parentModels", "GetParentModelRoute", x => new { parentId = x.ParentId });
                .RequiresRoutedLink("subModels", "GetSubModelsRoute", x => new { id = x.Id });
                .RequiresRoutedLink("delete", "DeleteModelRoute", x => new { id = x.Id });
      });
    });
  }
}
```

With a named policy, this can be applied at runtime using an overload of ```AddLinksAsync``` which takes a policy name:

```csharp
await linksService.AddLinksAsync(model,"FullInfo");
```

You can also markup your controller method with a ```LinksAttribute``` to override the default policy applied. The below code would apply the "FullInfo" profile to the returned model without having to specify the policy name in the call to ```AddLinksAsync```.

```csharp
[Route("api/[controller]")]
public class MyController : Controller
{
    private readonly ILinksService linksService;
    
    public MyController(ILinksService linksService)
    {
        this.linksService = linksService;
    }
   
    [HttpGet("{id}",Name = "GetModelRoute")]
    [Links(Policy = "FullInfo")]
    public async Task<MyModel> GetMyModel(int id)
    {
         var model = await myRepository.GetMyModel(id);
         await linksService.AddLinksAsync(model);
         return model;
    }
}
```

Another way to achieve the same thing is to mark the actual object with the `LinksAttribute`:

```csharp
[Links(Policy="FullInfo")]
public class MyModel : LinksContainer
{ }

[Route("api/[controller]")]
public class MyController : Controller
{
    private readonly ILinksService linksService;
    
    public MyController(ILinksService linksService)
    {
        this.linksService = linksService;
    }
   
    [HttpGet("{id}",Name = "GetModelRoute")]
    public async Task<MyModel> GetMyModel(int id)
    {
         MyModel model = await myRepository.GetMyModel(id);
         await linksService.AddLinksAsync(model);
         return model;
    }
}
```

There are further overloads of `AddLinksAsync` which take an instance of  [`ILinksPolicy`](src/RiskFirst.Hateoas/ILinksPolicy.cs) or an array of [`ILinksRequirement`](src/RiskFirst.Hateoas/ILinksRequirement.cs) which will be evaluated at runtime. This should give complete control of which links are applied at any point within your api code.

### Configuring Href and Rel transformations

There should not have much need to change how the `Href` is transformed, however one common requirement is to output relative instead of absolute uris. This can be tried in the [Basic Sample](Samples/RiskFirst.Hateoas.BasicSample)

```csharp
services.AddLinks(config => 
{
  config.UseRelativeHrefs();
  ...
});
```

Both Href and Rel transformations can be fully controlled by supplying a class or Type which implements [`ILinkTransformation`](src/RiskFirst.Hateoas/ILinkTransformation.cs).

```csharp
services.AddLinks(config => 
{
  // supply a type implementing ILinkTransformation
  config.UseHrefTransformation<MyHrefTransformation>();
  // or supply an instance
  config.UseRelTransformation(new MyRelTransformation());
});
```

Alternatively, transformations can be configured using a builder syntax

```csharp
services.AddLinks(config => 
{
  // output a uri for the rel values
  config.ConfigureRelTransformation(transform => transform.AddProtocol()
                                                          .AddHost()
                                                          .AddVirtualPath(ctx => $"/rel/{ctx.LinkSpec.ControllerName}/{ctx.LinkSpec.RouteName}");
});
```
Both ways of customizaing transformations can be seen in the [LinkConfigurationSample](samples/RiskFirst.Hateoas.LinkConfigurationSample).

### Authorization and Conditional links

It is likely that you wish to control which links are included with each model, and one common requirement is to only show links for which the current user is authorized. This library fully integrates into the authorization pipeline and will apply any authorization policy you have applied to the linked action.

To enable authorization on a link provide the `AuthorizeRoute` condition.

```csharp
public class Startup
{
  public void ConfigureServices(IServicesCollection services)
  {
    services.AddLinks(config => 
    {      
      config.AddPolicy<MyModel>("FullInfo",policy => {
          policy.RequiresSelfLink()
                .RequiresRoutedLink("all", "GetAllModelsRoute")
                .RequiresRoutedLink("parentModels", "GetParentModelRoute", 
                                      x => new { parentId = x.ParentId }, condition => condition.AuthorizeRoute());
                .RequiresRoutedLink("subModels", "GetSubModelsRoute", 
                                      x => new { id = x.Id }, condition => condition.AuthorizeRoute());
                .RequiresRoutedLink("delete", "DeleteModelRoute", 
                                      x => new { id = x.Id }, condition => condition.AuthorizeRoute());
      });
    });
  }
}
```

In the above example, `GetParentModelRoute`, `GetSubModelsRoute` & `DeleteModelRoute` will not be shown to a user who does not have access to those routes as defined by their authorization policies. See the [Microsoft documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/) for more information on authrization within an aspnet core webapi project.

As with the above examples, there are further condition methods which allow you to specifiy a policy name, an absolute policy or a set of requirements.

You can also conditionally show a link based on any boolean logic by using the `Assert` condition. For example, there is a method which allows you to add common paging links to paged results of objects. You may decide these are not worthwhile if there is a total of only one page of results.

```csharp
options.AddPolicy<IPageLinkContainer>(policy =>
{
    policy.RequireSelfLink("all")
            .RequiresPagingLinks(condition => condition.Assert(x => x.PageCount > 1 ));
});
```

### Further customization

You are free to add your own requirements using the generic `Requires` method on `ILinkPolicyBuilder`. In addition, you must write an implementation of `ILinksHandler` to handle your requirement. For example, you may have a requirement on certain responses to provide a link back to your api root document. Define a simple requirement for this link.

```csharp
using RiskFirst.Hateoas;

public class ApiRootLinkRequirement : ILinksRequirement
{
    public ApiRootLinkRequirement()
    {
    }
    public string Id { get; set; } = "root";
}
```
Given this requirement, we need a class to handle it, which must implement `ILinkHandler` and handle your requirement.

```csharp
using RiskFirst.Hateoas;

public class ApiRootLinkHandler : LinksHandler<ApiRootLinkRequirement>
{
    protected override Task HandleRequirementAsync(LinksHandlerContext context, ApiRootLinkRequirement requirement)
    {
        var route = context.RouteMap.GetRoute("ApiRoot"); // Assumes your controller has a named route "ApiRoot".
        context.Links.Add(new LinkSpec(requirement.Id, route));
        context.Handled(requirement);
        return Task.CompletedTask;
    }
}
```

Finally register your Handler with `IServicesCollection` and use the requirement within your link policy

```csharp
public class Startup
{
  public void ConfigureServices(IServicesCollection services)
  {   
    services.AddLinks(config => 
    {
      config.AddPolicy<MyModel>(policy => 
      {
          policy.RequiresRoutedLink("self","GetModelRoute", x => new {id = x.Id })
                .Requires<ApiRootLinkRequirement>();
      });
    });
    
    services.AddTransient<ILinksHandler,ApiRootLinkHandler>();
  }
}
```

This example is demonstrated in the [`CustomRequirementSample`](samples/RiskFirst.Hateoas.CustomRequirementSample)

There are many additional parts of the framework which can be extended by writing your own implementation of the appropriate interface and registering it with `IServicesCollection` for dependency injection. For example, you could change the way that links are evaluated and applied to your link container by implementing your own [`ILinksEvaluator`](src/RiskFirst.Hateoas/ILinksEvaluator.cs)

```csharp
using RiskFirst.Hateoas;

public class Startup
{
  public void ConfigureServices(IServicesCollection services)
  {
    services.AddLinks(options => {
        ...
    });
    services.AddTransient<ILinksEvaluator, MyLinksEvaluator>();
  }
}
```

The list of interfaces which have a default implementation, but which can be replaced is:

- [`ILinkAuthorizationService`](src/RiskFirst.Hateoas/ILinkAuthorizationService.cs), 
controls how links are authorized during link condition evaluation.
- [`ILinksEvaluator`](src/RiskFirst.Hateoas/ILinksEvaluator.cs), controls how links are evaluated and transformed before being written to the returned model.
- [`ILinksHandlerContextFactory`](src/RiskFirst.Hateoas/ILinksHandlerContextFactory.cs), controls how the context is created which is passed through the requirement handlers during processing.
- [`ILinksPolicyProvider`](src/RiskFirst.Hateoas/ILinksPolicyProvider.cs), provides lookup for `ILinkPolicy` instances by resource type and name.
- [`ILinksService`](src/RiskFirst.Hateoas/ILinksService.cs), the main entrypoint into the framework, this interface is injected into user code to apply links to api resources. 
- [`ILinkTransformationContextFactory`](src/RiskFirst.Hateoas/ILinkTransformationContextFactory.cs), controls how the transformation context is created during transformation for rel & href properies of links.
- [`IRouteMap`](src/RiskFirst.Hateoas/IRouteMap.cs), controls how your API is indexed to allow links between routes.

### Troubleshooting

#### Upgrading from v1.0.x to v1.1.x

The change from version 1.0.x to 1.1.x was mostly non-breaking, however if you have implemented any custom requirement handlers as described in the example above the signature of the base class `LinksHandler` changed slightly to remove the duplicate declaration of the generic type `TResource`. 

In v1.0.x your code may have looked like:

```csharp
public class MyCustomHandler : ILinksHandler { ... }
```

It should now inherit from `LinksHandler<TRequirement>` making implementation simpler, and giving a type-safe override of `HandleRequirementAsync` giving access to your correctly-typed requirement.

```csharp
public class MyCustomHandler : LinksHandler<MyCustomRequirement>
```
