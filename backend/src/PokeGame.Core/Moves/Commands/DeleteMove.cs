using Krakenar.Core;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Core.Moves.Commands;

internal record DeleteMove(Guid Id) : ICommand<MoveModel?>;

internal class DeleteMoveHandler : ICommandHandler<DeleteMove, MoveModel?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMoveQuerier _moveQuerier;
  private readonly IMoveRepository _moveRepository;

  public DeleteMoveHandler(IApplicationContext applicationContext, IMoveQuerier moveQuerier, IMoveRepository moveRepository)
  {
    _applicationContext = applicationContext;
    _moveQuerier = moveQuerier;
    _moveRepository = moveRepository;
  }

  public async Task<MoveModel?> HandleAsync(DeleteMove command, CancellationToken cancellationToken)
  {
    MoveId moveId = new(command.Id);
    Move? move = await _moveRepository.LoadAsync(moveId, cancellationToken);
    if (move is null)
    {
      return null;
    }
    MoveModel model = await _moveQuerier.ReadAsync(move, cancellationToken);

    move.Delete(_applicationContext.ActorId);
    await _moveRepository.SaveAsync(move, cancellationToken);

    return model;
  }
}
