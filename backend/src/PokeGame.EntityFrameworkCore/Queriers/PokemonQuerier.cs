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
  private readonly DbSet<FormEntity> _forms;
  private readonly DbSet<PokemonEntity> _pokemon;

  public PokemonQuerier(IActorService actorService, PokemonContext context)
  {
    _actorService = actorService;
    _forms = context.Forms;
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
      .Include(x => x.CurrentTrainer)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .Include(x => x.OriginalTrainer)
      .Include(x => x.PokeBall)
      .SingleOrDefaultAsync(x => x.StreamId == id.Value, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    await FillAsync(pokemon, cancellationToken);

    return await MapAsync(pokemon, cancellationToken);
  }
  public async Task<PokemonModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.CurrentTrainer)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .Include(x => x.OriginalTrainer)
      .Include(x => x.PokeBall)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    await FillAsync(pokemon, cancellationToken);

    return await MapAsync(pokemon, cancellationToken);
  }
  public async Task<PokemonModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    PokemonEntity? pokemon = await _pokemon.AsNoTracking()
      .Include(x => x.CurrentTrainer)
      .Include(x => x.HeldItem).ThenInclude(x => x!.Move)
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .Include(x => x.OriginalTrainer)
      .Include(x => x.PokeBall)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    await FillAsync(pokemon, cancellationToken);

    return await MapAsync(pokemon, cancellationToken);
  }

  private async Task FillAsync(PokemonEntity pokemon, CancellationToken cancellationToken)
  {
    FormEntity form = await _forms.AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Species).ThenInclude(x => x!.RegionalNumbers).ThenInclude(x => x.Region)
      .SingleOrDefaultAsync(x => x.FormId == pokemon.FormId, cancellationToken)
      ?? throw new InvalidOperationException($"The Pokémon form entity 'FormId={pokemon.FormId}' was not found.");
    pokemon.Form = form;
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
