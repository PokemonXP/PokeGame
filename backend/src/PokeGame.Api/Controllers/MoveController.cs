using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Move;
using PokeGame.Core.Moves;
using PokeGame.Core.Moves.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("api/moves")]
public class MoveController : ControllerBase
{
  private readonly IMoveQuerier _moveQuerier;

  public MoveController(IMoveQuerier moveQuerier)
  {
    _moveQuerier = moveQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<MoveModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    MoveModel? move = await _moveQuerier.ReadAsync(id, cancellationToken);
    return move is null ? NotFound() : Ok(move);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<MoveModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    MoveModel? move = await _moveQuerier.ReadAsync(uniqueName, cancellationToken);
    return move is null ? NotFound() : Ok(move);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<MoveModel>>> SearchAsync([FromQuery] SearchMovesParameters parameters, CancellationToken cancellationToken)
  {
    SearchMovesPayload payload = parameters.ToPayload();
    SearchResults<MoveModel> moves = await _moveQuerier.SearchAsync(payload, cancellationToken);
    return Ok(moves);
  }
}
