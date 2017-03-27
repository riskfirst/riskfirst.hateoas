using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RiskFirst.Hateoas.BasicSample.Models;
using RiskFirst.Hateoas.Models;
using RiskFirst.Hateoas.BasicSample.Repository;

namespace RiskFirst.Hateoas.BasicSample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IValuesRepository repo;
        private readonly ILinksService linksService;

        public ValuesController(IValuesRepository repo, ILinksService linksService)
        {
            this.repo = repo;
            this.linksService = linksService;
        }

        // GET api/values
        [HttpGet( Name = "GetAllValuesRoute")]
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
        [Links(Policy = "FullInfoPolicy")]
        public async Task<ValueInfo> Get(int id)
        {
            var value = await repo.GetValueAsync(id);
            await linksService.AddLinksAsync(value);
            return value;
        }

        // POST api/values
        [HttpPost(Name = "InsertValueRoute")]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}",Name = "UpdateValueRoute")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}",Name = "DeleteValueRoute")]
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
}
