using Krakenar.Core;
using PokeGame.Core.Abilities.Models;

namespace PokeGame.Core.Abilities.Commands;

internal record DeleteAbility(Guid Id) : ICommand<AbilityModel?>;

internal class DeleteAbilityHandler : ICommandHandler<DeleteAbility, AbilityModel?>
{
  private readonly IAbilityQuerier _abilityQuerier;
  private readonly IAbilityRepository _abilityRepository;
  private readonly IApplicationContext _applicationContext;

  public DeleteAbilityHandler(IAbilityQuerier abilityQuerier, IAbilityRepository abilityRepository, IApplicationContext applicationContext)
  {
    _abilityQuerier = abilityQuerier;
    _abilityRepository = abilityRepository;
    _applicationContext = applicationContext;
  }

  public async Task<AbilityModel?> HandleAsync(DeleteAbility command, CancellationToken cancellationToken)
  {
    AbilityId abilityId = new(command.Id);
    Ability? ability = await _abilityRepository.LoadAsync(abilityId, cancellationToken);
    if (ability is null)
    {
      return null;
    }
    AbilityModel model = await _abilityQuerier.ReadAsync(ability, cancellationToken);

    ability.Delete(_applicationContext.ActorId);
    await _abilityRepository.SaveAsync(ability, cancellationToken);

    return model;
  }
}
