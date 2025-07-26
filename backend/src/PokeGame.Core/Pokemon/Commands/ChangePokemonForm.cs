using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core;
using Logitar.EventSourcing;
using PokeGame.Core.Forms;
using PokeGame.Core.Pokemon.Models;
using PokeGame.Core.Pokemon.Validators;
using PokeGame.Core.Varieties;

namespace PokeGame.Core.Pokemon.Commands;

internal record ChangePokemonForm(Guid Id, ChangePokemonFormPayload Payload) : ICommand<PokemonModel?>;

/// <exception cref="FormNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
internal class ChangePokemonFormHandler : ICommandHandler<ChangePokemonForm, PokemonModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IFormRepository _formRepository;
  private readonly IPokemonQuerier _pokemonQuerier;
  private readonly IPokemonRepository _pokemonRepository;
  private readonly IVarietyRepository _varietyRepository;

  public ChangePokemonFormHandler(
    IApplicationContext applicationContext,
    IFormRepository formRepository,
    IPokemonQuerier pokemonQuerier,
    IPokemonRepository pokemonRepository,
    IVarietyRepository varietyRepository)
  {
    _applicationContext = applicationContext;
    _formRepository = formRepository;
    _pokemonQuerier = pokemonQuerier;
    _pokemonRepository = pokemonRepository;
    _varietyRepository = varietyRepository;
  }

  public async Task<PokemonModel?> HandleAsync(ChangePokemonForm command, CancellationToken cancellationToken)
  {
    ActorId? actorId = _applicationContext.ActorId;

    ChangePokemonFormPayload payload = command.Payload;
    new ChangePokemonFormValidator().ValidateAndThrow(payload);

    PokemonId pokemonId = new(command.Id);
    Specimen? pokemon = await _pokemonRepository.LoadAsync(pokemonId, cancellationToken);
    if (pokemon is null)
    {
      return null;
    }

    Form target = await _formRepository.LoadAsync(payload.Form, cancellationToken) ?? throw new FormNotFoundException(payload.Form, nameof(payload.Form));
    if (target.Id != pokemon.FormId)
    {
      if (pokemon.VarietyId != target.VarietyId)
      {
        ValidationFailure failure = new(nameof(payload.Form), "The Pokémon current and target form varieties are not the same.", payload.Form)
        {
          ErrorCode = "InvalidVariety"
        };
        throw new ValidationException([failure]);
      }

      Variety variety = await _varietyRepository.LoadAsync(pokemon.VarietyId, cancellationToken)
        ?? throw new InvalidOperationException($"The variety 'Id={pokemon.VarietyId}' was not found.");
      if (!variety.CanChangeForm)
      {
        ValidationFailure failure = new(nameof(payload.Form), "The Pokémon variety cannot change form.", payload.Form)
        {
          ErrorCode = "VarietyCannotChangeForm"
        };
        throw new ValidationException([failure]);
      }

      if (target.IsBattleOnly)
      {
        // TASK: [POKEGAME-211](https://logitar.atlassian.net/browse/POKEGAME-211)
      }
      if (target.IsMega)
      {
        // TASK: [POKEGAME-212](https://logitar.atlassian.net/browse/POKEGAME-212)
      }

      pokemon.ChangeForm(target, actorId);

      await _pokemonRepository.SaveAsync(pokemon, cancellationToken);
    }

    return await _pokemonQuerier.ReadAsync(pokemon, cancellationToken);
  }
}
