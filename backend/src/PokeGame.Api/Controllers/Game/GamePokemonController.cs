using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Extensions;
using PokeGame.Api.Models.Game;
using PokeGame.Core.Pokemon;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Controllers.Game;

[ApiController]
[Authorize]
[Route("game/pokemon")]
public class GamePokemonController : ControllerBase
{
  private readonly IPokemonService _pokemonService;
  private readonly ITrainerService _trainerService;

  public GamePokemonController(IPokemonService pokemonService, ITrainerService trainerService)
  {
    _pokemonService = pokemonService;
    _trainerService = trainerService;
  }

  [HttpGet("/game/trainers/{trainerId}/pokemon")]
  public async Task<ActionResult<PokemonSheet[]>> GetAsync(Guid trainerId, int? box, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.ReadAsync(trainerId, uniqueName: null, license: null, cancellationToken);
    if (trainer is null)
    {
      return NotFound();
    }
    else if (trainer.UserId != HttpContext.GetUserId())
    {
      return Forbid();
    }

    SearchPokemonPayload payload = new()
    {
      TrainerId = trainer.Id,
      InParty = !box.HasValue,
      Box = box
    };
    payload.Sort.Add(new PokemonSortOption(PokemonSort.Position));
    SearchResults<PokemonModel> results = await _pokemonService.SearchAsync(payload, cancellationToken);

    PokemonSheet[] pokemon = results.Items.Select(pokemon => new PokemonSheet(pokemon)).ToArray();
    return Ok(pokemon);
  }
}
