using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Models.Trainer;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize] // TODO(fpion): admin
[Route("api/trainers")]
public class TrainerController : ControllerBase
{
  private readonly ITrainerQuerier _trainerQuerier;

  public TrainerController(ITrainerQuerier trainerQuerier)
  {
    _trainerQuerier = trainerQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TrainerModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerQuerier.ReadAsync(id, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<TrainerModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerQuerier.ReadAsync(uniqueName, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  [HttpGet("license:{license}")]
  public async Task<ActionResult<TrainerModel>> ReadByLicenseAsync(string license, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerQuerier.ReadByLicenseAsync(license, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<TrainerModel>>> SearchAsync([FromQuery] SearchTrainersParameters parameters, CancellationToken cancellationToken)
  {
    SearchTrainersPayload payload = parameters.ToPayload();
    SearchResults<TrainerModel> trainers = await _trainerQuerier.SearchAsync(payload, cancellationToken);
    return Ok(trainers);
  }
}
