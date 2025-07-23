using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Trainers;

namespace PokeGame.Core.Pokemon.Commands;

internal record DepositPokemon(Guid Id) : ICommand<PokemonModel?>;

/// <exception cref="CannotEmptyTrainerPartyException"></exception>
internal class DepositPokemonHandler : ICommandHandler<DepositPokemon, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;

  public DepositPokemonHandler(IApplicationContext applicationContext, IPokemonQuerier pokemonQuerier, IPokemonRepository pokemonRepository)
  {
    _applicationContext = applicationContext;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
  }

  public async Task<PokemonModel?> HandleAsync(DepositPokemon command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    if (pokemon.Ownership is not null && pokemon.Slot is not null && pokemon.Slot.Box is null)
    {
      TrainerId trainerId = pokemon.Ownership.TrainerId;
      Storage storage = await _pokemonQuerier.GetStorageAsync(trainerId, cancellationToken);
      if (!pokemon.IsEgg)
      {
        IEnumerable<PokemonId> pokemonIds = storage.Party.Except([pokemon.Id]);
        IReadOnlyCollection<Specimen> partyPokemon = await _pokemonRepository.LoadAsync(pokemonIds, cancellationToken);
        if (!partyPokemon.Any(p => !p.IsEgg))
        {
          throw new CannotEmptyTrainerPartyException(trainerId);
        }
      }

      PokemonSlot slot = storage.GetFirstBoxEmptySlot();
      pokemon.Deposit(slot, actorId);

      await _pokemonRepository.SaveAsync(pokemon, cancellationToken);
    }

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
