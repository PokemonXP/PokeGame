using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Models.Index;
using PokeGame.Api.Settings;

namespace PokeGame.Api.Controllers;

[ApiController]
[Route("")]
public class IndexController : ControllerBase
{
  private readonly ApiSettings _settings;

  public IndexController(ApiSettings settings)
  {
    _settings = settings;
  }

  [HttpGet]
  public ActionResult<ApiVersion> Get() => new ApiVersion(_settings.Title, _settings.Version);
}
