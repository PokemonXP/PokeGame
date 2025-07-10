using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
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
}
