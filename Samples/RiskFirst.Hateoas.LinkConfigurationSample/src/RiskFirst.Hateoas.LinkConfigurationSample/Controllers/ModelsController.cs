using Microsoft.AspNetCore.Mvc;

namespace RiskFirst.Hateoas.LinkConfigurationSample.Controllers;

[ApiController]
[Route("models")]
public class ModelsController : ControllerBase
{
    [HttpGet("{model}")]
    public object GetEntity(string model)
    {
        var type = model.Replace("-", ".");
        return type;
    }
    [HttpGet("{container}/of/{model}")]
    public object GetContainerEntity(string container, string model)
    {
        var containerType = container.Replace("-", ".");
        var modelType = model.Replace("-", ".");
        return $"{containerType}<{modelType}>";
    }
}
