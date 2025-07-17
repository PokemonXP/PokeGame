using Krakenar.Core;
using PokeGame.Core.Varieties.Models;

namespace PokeGame.Core.Varieties.Commands;

internal record DeleteVariety(Guid Id) : ICommand<VarietyModel?>;

internal class DeleteVarietyHandler : ICommandHandler<DeleteVariety, VarietyModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IVarietyQuerier _varietyQuerier;
  private readonly IVarietyRepository _varietyRepository;

  public DeleteVarietyHandler(IApplicationContext applicationContext, IVarietyQuerier varietyQuerier, IVarietyRepository varietyRepository)
  {
    _applicationContext = applicationContext;
    _varietyQuerier = varietyQuerier;
    _varietyRepository = varietyRepository;
  }

  public async Task<VarietyModel?> HandleAsync(DeleteVariety command, CancellationToken cancellationToken)
  {
    VarietyId varietyId = new(command.Id);
    Variety? variety = await _varietyRepository.LoadAsync(varietyId, cancellationToken);
    if (variety is null)
    {
      return null;
    }
    VarietyModel model = await _varietyQuerier.ReadAsync(variety, cancellationToken);

    variety.Delete(_applicationContext.ActorId);
    await _varietyRepository.SaveAsync(variety, cancellationToken);

    return model;
  }
}
