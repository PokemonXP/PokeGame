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
  private readonly IFormService _formService;

  public FormController(IFormService formService)
  {
    _formService = formService;
  }

  [HttpPost]
  public async Task<ActionResult<FormModel>> CreateAsync([FromBody] CreateOrReplaceFormPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceFormResult result = await _formService.CreateOrReplaceAsync(payload, id: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<FormModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    FormModel? form = await _formService.DeleteAsync(id, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<FormModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FormModel? form = await _formService.ReadAsync(id, uniqueName: null, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  [HttpGet("name:{uniqueName}")]
  public async Task<ActionResult<FormModel>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    FormModel? form = await _formService.ReadAsync(id: null, uniqueName, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<FormModel>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceFormPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceFormResult result = await _formService.CreateOrReplaceAsync(payload, id, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<FormModel>>> SearchAsync([FromQuery] SearchFormsParameters parameters, CancellationToken cancellationToken)
  {
    SearchFormsPayload payload = parameters.ToPayload();
    SearchResults<FormModel> forms = await _formService.SearchAsync(payload, cancellationToken);
    return Ok(forms);
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<FormModel>> UpdateAsync(Guid id, [FromBody] UpdateFormPayload payload, CancellationToken cancellationToken)
  {
    FormModel? form = await _formService.UpdateAsync(id, payload, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  private ActionResult<FormModel> ToActionResult(CreateOrReplaceFormResult result)
  {
    if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/forms/{result.Form.Id}", UriKind.Absolute);
      return Created(location, result.Form);
    }

    return Ok(result.Form);
  }
}
