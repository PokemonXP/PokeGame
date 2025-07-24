using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Extensions;
using PokeGame.Api.Models.Game;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Controllers.Game;

[ApiController]
[Authorize]
[Route("game/trainers")]
public class GameTrainerController : ControllerBase
{
  private readonly ITrainerService _trainerService;

  public GameTrainerController(ITrainerService trainerService)
  {
    _trainerService = trainerService;
  }

  [HttpGet]
  public async Task<ActionResult<TrainerSheet[]>> GetAsync(CancellationToken cancellationToken)
  {
    SearchTrainersPayload payload = new()
    {
      UserId = HttpContext.GetUserId()
    };
    payload.Sort.Add(new TrainerSortOption(TrainerSort.DisplayName));
    SearchResults<TrainerModel> results = await _trainerService.SearchAsync(payload, cancellationToken);

    TrainerSheet[] trainers = results.Items.Select(trainer => new TrainerSheet(trainer)).ToArray();
    return Ok(trainers);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TrainerSheet>> GetAsync(Guid id, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.ReadAsync(id, uniqueName: null, license: null, cancellationToken);
    if (trainer is null)
    {
      return NotFound();
    }
    else if (trainer.UserId != HttpContext.GetUserId())
    {
      return Forbid();
    }

    TrainerSheet sheet = new(trainer);
    return Ok(sheet);
  }
}
