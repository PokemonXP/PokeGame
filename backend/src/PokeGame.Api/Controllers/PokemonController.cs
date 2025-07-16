using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Pokemon;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("pokemon")]
public class PokemonController : ControllerBase
{
  private readonly IPokemonService _pokemonService;

  public PokemonController(IPokemonService pokemonService)
  {
    _pokemonService = pokemonService;
  }

  [HttpPost]
  public async Task<ActionResult<PokemonModel>> CreateAsync([FromBody] CreatePokemonPayload payload, CancellationToken cancellationToken)
  {
    PokemonModel pokemon = await _pokemonService.CreateAsync(payload, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/pokemon/{pokemon.Id}", UriKind.Absolute);
    return Created(location, pokemon);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<PokemonModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    PokemonModel? pokemon = await _pokemonService.DeleteAsync(id, cancellationToken);
    return pokemon is null ? NotFound() : Ok(pokemon);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PokemonModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PokemonModel? pokemon = await _pokemonService.ReadAsync(id, uniqueName: null, cancellationToken);
    return pokemon is null ? NotFound() : Ok(pokemon);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<PokemonModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    PokemonModel? pokemon = await _pokemonService.ReadAsync(id: null, uniqueName, cancellationToken);
    return pokemon is null ? NotFound() : Ok(pokemon);
  }

  [HttpPut("{id}/moves/relearn")]
  public async Task<ActionResult<PokemonModel>> RelearnMoveAsync(Guid id, [FromBody] RelearnPokemonMovePayload payload, CancellationToken cancellationToken)
  {
    PokemonModel? pokemon = await _pokemonService.RelearnMoveAsync(id, payload, cancellationToken);
    return pokemon is null ? NotFound() : Ok(pokemon);
  }

  [HttpPut("{id}/moves/switch")]
  public async Task<ActionResult<PokemonModel>> SwitchMovesAsync(Guid id, [FromBody] SwitchPokemonMovesPayload payload, CancellationToken cancellationToken)
  {
    PokemonModel? pokemon = await _pokemonService.SwitchMovesAsync(id, payload, cancellationToken);
    return pokemon is null ? NotFound() : Ok(pokemon);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PokemonModel>>> SearchAsync([FromQuery] SearchPokemonParameters parameters, CancellationToken cancellationToken)
  {
    SearchPokemonPayload payload = parameters.ToPayload();
    SearchResults<PokemonModel> regions = await _pokemonService.SearchAsync(payload, cancellationToken);
    return Ok(regions);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<PokemonModel>> UpdateAsync(Guid id, [FromBody] UpdatePokemonPayload payload, CancellationToken cancellationToken)
  {
    PokemonModel? pokemon = await _pokemonService.UpdateAsync(id, payload, cancellationToken);
    return pokemon is null ? NotFound() : Ok(pokemon);
  }
}
