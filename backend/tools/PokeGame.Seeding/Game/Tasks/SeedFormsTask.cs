using MediatR;
using PokeGame.Core.Forms;
using PokeGame.Core.Forms.Models;
using PokeGame.Tools.Shared;
using PokeGame.Tools.Shared.Models;

namespace PokeGame.Seeding.Game.Tasks;

internal class SeedFormsTask : SeedingTask
{
  public override string? Description => "Seeds Form contents into Krakenar.";
}

internal class SeedFormsTaskHandler : INotificationHandler<SeedFormsTask>
{
  private readonly IFormService _formService;
  private readonly ILogger<SeedFormsTaskHandler> _logger;

  public SeedFormsTaskHandler(IFormService formService, ILogger<SeedFormsTaskHandler> logger)
  {
    _formService = formService;
    _logger = logger;
  }

  public async Task Handle(SeedFormsTask task, CancellationToken cancellationToken)
  {
    CsvManager csv = new([new SeedFormPayload.Map()]);
    IReadOnlyCollection<SeedFormPayload> forms = await csv.ExtractAsync<SeedFormPayload>("Game/data/forms.csv", cancellationToken);
    foreach (SeedFormPayload form in forms)
    {
      CreateOrReplaceFormResult result = await _formService.CreateOrReplaceAsync(form, form.Id, cancellationToken);
      _logger.LogInformation("The form '{Form}' was {Status}.", result.Form, result.Created ? "created" : "updated");
    }
  }
}
