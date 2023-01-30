using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas.BasicSample.Models;
using RiskFirst.Hateoas.BasicSample.Repository;
using RiskFirst.Hateoas.Models;

namespace RiskFirst.Hateoas.BasicSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ValuesController : ControllerBase
{
    private readonly IValuesRepository repo;
    private readonly ILinksService linksService;

    public ValuesController(IValuesRepository repo, ILinksService linksService)
    {
        this.repo = repo;
        this.linksService = linksService;
    }

    [HttpGet(Name = "GetAllValuesRoute")]
    public async Task<ItemsLinkContainer<ValueInfo>> Get()
    {
        var values = await GetAllValuesWithLinksAsync();

        var result = new ItemsLinkContainer<ValueInfo>()
        {
            Items = values
        };
        await linksService.AddLinksAsync(result);
        return result;
    }

    // GET api/values/5
    [HttpGet("{id}", Name = "GetValueByIdRoute")]
    [HttpGet("v2/{id}", Name = "GetValueByIdRouteV2")]
    [Links(Policy = "FullInfoPolicy")]
    public async Task<ValueInfo> Get(int id)
    {
        var value = await repo.GetValueAsync(id);
        await linksService.AddLinksAsync(value);
        return value;
    }

    // POST api/values
    [HttpPost(Name = "InsertValueRoute")]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/values/5
    [HttpPut("{id}", Name = "UpdateValueRoute")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/values/5
    [HttpDelete("{id}", Name = "DeleteValueRoute")]
    public void Delete(int id)
    {
    }

    private async Task<List<ValueInfo>> GetAllValuesWithLinksAsync()
    {
        var values = await repo.GetAllValuesAsync();
        values.ForEach(async x => await linksService.AddLinksAsync(x));
        return values; ;
    }
}
