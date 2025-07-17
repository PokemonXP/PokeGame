using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Trainer;
using PokeGame.Core.Trainers;
using PokeGame.Core.Trainers.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("trainers")]
public class TrainerController : ControllerBase
{
  private readonly ITrainerService _trainerService;

  public TrainerController(ITrainerService trainerService)
  {
    _trainerService = trainerService;
  }

  [HttpPost]
  public async Task<ActionResult<TrainerModel>> CreateAsync([FromBody] CreateOrReplaceTrainerPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTrainerResult result = await _trainerService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<TrainerModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.DeleteAsync(id, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<TrainerModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.ReadAsync(id, uniqueName: null, license: null, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<TrainerModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.ReadAsync(id: null, uniqueName, license: null, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  [HttpGet("license:{license}")]
  public async Task<ActionResult<TrainerModel>> ReadByLicenseAsync(string license, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.ReadAsync(id: null, uniqueName: null, license, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TrainerModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceTrainerPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTrainerResult result = await _trainerService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<TrainerModel>>> SearchAsync([FromQuery] SearchTrainersParameters parameters, CancellationToken cancellationToken)
  {
    SearchTrainersPayload payload = parameters.ToPayload();
    SearchResults<TrainerModel> trainers = await _trainerService.SearchAsync(payload, cancellationToken);
    return Ok(trainers);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<TrainerModel>> UpdateAsync(Guid id, [FromBody] UpdateTrainerPayload payload, CancellationToken cancellationToken)
  {
    TrainerModel? trainer = await _trainerService.UpdateAsync(id, payload, cancellationToken);
    return trainer is null ? NotFound() : Ok(trainer);
  }

  private ActionResult<TrainerModel> ToActionResult(CreateOrReplaceTrainerResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/trainers/{result.Trainer.Id}", UriKind.Absolute);
      return Created(location, result.Trainer);
    }

    return Ok(result.Trainer);
  }
}
