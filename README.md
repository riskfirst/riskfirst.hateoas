# riskfirst.hateos
An implementation of [HATEOAS](https://en.wikipedia.org/wiki/HATEOAS) for aspnet core web api projects which gives full control of which links to apply to models returned from your api. IN order to communicate varying state to the end-user, this library fully integrates with Authorization, and allows arbitrary conditions to determine whether to show or hide HATEOAS links between api resources.

### Getting started

Install the package

```powershell
PM> Install-Package {{TBC}}
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

There are further overloads of ```AddLinksAsync``` which take an instance of  [ILinksPolicy](https://github.com/riskfirst/riskfirst.hateoas/blob/master/src/RiskFirst.Hateoas/ILinksPolicy.cs) or an array of [ILinksRequirement](https://github.com/riskfirst/riskfirst.hateoas/blob/master/src/RiskFirst.Hateoas/ILinksRequirement.cs) which will be evaluated at runtime. This should give complete control of which links are applied at any point within your api code.

### Authorization and Conditional links

It is likely that you wish to control which links are included with each model, and one common requirement is to only show links for which the current user is authorized. This library fully integrates into the authorization pipeline and will apply any authorization policy you have applied to the current controller or action.

To enable authorization on a link provide the ```AuthorizeRoute``` condition.

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
                .RequiresRoutedLink("parentModels", "GetParentModelRoute", x => new { parentId = x.ParentId }, condition => condition.AuthorizeRoute());
                .RequiresRoutedLink("subModels", "GetSubModelsRoute", x => new { id = x.Id }, condition => condition.AuthorizeRoute());
                .RequiresRoutedLink("delete", "DeleteModelRoute", x => new { id = x.Id }, condition => condition.AuthorizeRoute());
      });
    });
  }
}
```

In the above example, ```GetParentModelRoute```, ```GetSubModelsRoute``` & ```DeleteModelRoute``` will not be shown to a user who does not have access to those routes as defined by their authorization policies. See the [Microsoft documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/) for more information on authrization within an aspnet core webapi project.

As with the above examples, there are further condition methods which allow you to specifiy a policy name, an absolute policy or a set of requirements.

You can also conditionally show a link based on any boolean logic by using the `Assert` condition. For example, there is a method which allows you to add common paging links to paged results of objects. You may decide these are not worthwhile if there is a total of only one page of results.

```csharp
options.AddPolicy<IPageLinkContainer>(policy =>
{
    policy.RequireSelfLink("all")
            .RequiresPagingLinks(condition => condition.Assert(x => x.PageCount > 1 ));
});
```

### Troubleshooting

Nothing here yet!
