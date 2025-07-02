using Krakenar.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Cms.Models.Index;

namespace PokeGame.Cms.Controllers;

[ApiController]
[Route("api")]
public class IndexController : ControllerBase
{
  private readonly AdminSettings _adminSettings;

  public IndexController(AdminSettings adminSettings)
  {
    _adminSettings = adminSettings;
  }

  [HttpGet]
  public ActionResult<ApiVersion> Get()
  {
    ApiVersion version = new(_adminSettings.Title, _adminSettings.Version);
    return Ok(version);
  }
}

