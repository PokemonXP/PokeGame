using Krakenar.Contracts.Actors;
using Krakenar.Core;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeGame.Core.Pokemons;
using PokeGame.Core.Pokemons.Models;
using PokeGame.EntityFrameworkCore.Entities;

namespace PokeGame.EntityFrameworkCore.Queriers;

internal class PokemonQuerier : IPokemonQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<PokemonEntity> _pokemon;

  public PokemonQuerier(IActorService actorService, PokemonContext context)
  {
    _actorService = actorService;
    _pokemon = context.Pokemon;
  }

  public async Task<PokemonId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _pokemon.AsNoTracking()
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new PokemonId(streamId);
  }

  public async Task<PokemonModel> ReadAsync(Pokemon pokemon, CancellationToken cancellationToken)
  {
    return await ReadAsync(pokemon.Id, cancellationToken) ?? throw new InvalidOperationException($"The Pokémon entity 'StreamId={pokemon.Id}' was not found.");
  }
  public async Task<PokemonModel?> ReadAsync(PokemonId id, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.Form).ThenInclude(x => x!.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Form).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    return pokemon is null ? null : await MapAsync(pokemon, cancellationToken);
  }
  public async Task<PokemonModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.Form).ThenInclude(x => x!.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Form).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    return pokemon is null ? null : await MapAsync(pokemon, cancellationToken);
  }
  public async Task<PokemonModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.Form).ThenInclude(x => x!.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Form).ThenInclude(x => x!.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    return pokemon is null ? null : await MapAsync(pokemon, cancellationToken);
  }

  private async Task<PokemonModel> MapAsync(PokemonEntity pokemon, CancellationToken cancellationToken)
  {
    return (await MapAsync([pokemon], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<PokemonModel>> MapAsync(IEnumerable<PokemonEntity> pokemon, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = pokemon.SelectMany(p => p.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return pokemon.Select(mapper.ToPokemon).ToList().AsReadOnly();
  }
}
