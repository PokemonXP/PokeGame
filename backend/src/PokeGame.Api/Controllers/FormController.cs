using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PokeGame.Api.Constants;
using PokeGame.Api.Models.Form;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;

namespace PokeGame.Api.Controllers;

[ApiController]
[Authorize(Policy = Policies.IsAdmin)]
[Route("forms")]
public class FormController : ControllerBase
{
  private readonly IFormQuerier _formQuerier;

  public FormController(IFormQuerier formQuerier)
  {
    _formQuerier = formQuerier;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<FormModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FormModel? form = await _formQuerier.ReadAsync(id, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<FormModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    FormModel? form = await _formQuerier.ReadAsync(uniqueName, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  [HttpGet("/varieties/{varietyId}/forms")]
  public async Task<ActionResult<SearchResults<FormModel>>> SearchAsync(Guid varietyId, [FromQuery] SearchFormsParameters parameters, CancellationToken cancellationToken)
  {
    SearchFormsPayload payload = parameters.ToPayload();
    SearchResults<FormModel> forms = await _formQuerier.SearchAsync(varietyId, payload, cancellationToken);
    return Ok(forms);
  }
}
