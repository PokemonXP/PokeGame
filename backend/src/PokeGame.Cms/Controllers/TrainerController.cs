using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Cms.Models.Trainer;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Cms.Controllers;

[ApiController]
[Route("api/Trainers")]
public class TrainerController : ControllerBase
{
  private readonly ITrainerQuerier _TrainerQuerier;

  public TrainerController(ITrainerQuerier TrainerQuerier)
  {
    _TrainerQuerier = TrainerQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TrainerModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TrainerModel? Trainer = await _TrainerQuerier.ReadAsync(id, cancellationToken);
    return Trainer is null ? NotFound() : Ok(Trainer);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<TrainerModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    TrainerModel? Trainer = await _TrainerQuerier.ReadAsync(uniqueName, cancellationToken);
    return Trainer is null ? NotFound() : Ok(Trainer);
  }

  [HttpGet("license:{license}")]
  public async Task<ActionResult<TrainerModel>> ReadByLicenseAsync(string license, CancellationToken cancellationToken)
  {
    TrainerModel? Trainer = await _TrainerQuerier.ReadByLicenseAsync(license, cancellationToken);
    return Trainer is null ? NotFound() : Ok(Trainer);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<TrainerModel>>> SearchAsync([FromQuery] SearchTrainersParameters parameters, CancellationToken cancellationToken)
  {
    SearchTrainersPayload payload = parameters.ToPayload();
    SearchResults<TrainerModel> Trainers = await _TrainerQuerier.SearchAsync(payload, cancellationToken);
    return Ok(Trainers);
  }
}
