using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using MediatR;
using PokeGame.Seeding.Krakenar.Payloads;

namespace PokeGame.Seeding.Krakenar.Tasks;

internal class SeedContentTypesTask : SeedingTask
{
  public override string? Description => "Seeds the Content Types into Krakenar.";
  public bool FieldDefinitions { get; }

  public SeedContentTypesTask(bool fieldDefinitions = false)
  {
    FieldDefinitions = fieldDefinitions;
  }
}

internal class SeedContentTypesTaskHandler : INotificationHandler<SeedContentTypesTask>
{
  private readonly ILogger<SeedContentTypesTaskHandler> _logger;
  private readonly IContentTypeService _contentTypeService;
  private readonly IFieldDefinitionService _fieldDefinitionService;

  public SeedContentTypesTaskHandler(
    IContentTypeService contentTypeService,
    IFieldDefinitionService fieldDefinitionService,
    ILogger<SeedContentTypesTaskHandler> logger)
  {
    _contentTypeService = contentTypeService;
    _fieldDefinitionService = fieldDefinitionService;
    _logger = logger;
  }

  public async Task Handle(SeedContentTypesTask task, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("Krakenar/data/content_types.json", Encoding.UTF8, cancellationToken);
    IEnumerable<ContentTypePayload>? payloads = SeedingSerializer.Deserialize<IEnumerable<ContentTypePayload>>(json);
    if (payloads is not null)
    {
      foreach (ContentTypePayload payload in payloads)
      {
        if (task.FieldDefinitions)
        {
          ContentType contentType = await _contentTypeService.ReadAsync(payload.Id, uniqueName: null, cancellationToken)
            ?? throw new InvalidOperationException($"The content type 'Id={payload.Id}' was not found.");
          HashSet<Guid> fieldIds = payload.Fields.Select(x => x.Id).ToHashSet();
          foreach (FieldDefinition field in contentType.Fields)
          {
            if (!fieldIds.Contains(field.Id))
            {
              await _fieldDefinitionService.DeleteAsync(contentType.Id, field.Id, cancellationToken);
              _logger.LogInformation("The field definition '{FieldDefinition}' was deleted.", field.DisplayName ?? field.UniqueName);
            }
          }

          foreach (FieldDefinitionPayload field in payload.Fields)
          {
            await _fieldDefinitionService.CreateOrReplaceAsync(contentType.Id, field, field.Id, cancellationToken);
            _logger.LogInformation("The field definition '{FieldDefinition}' was saved.", field.DisplayName ?? field.UniqueName);
          }
        }
        else
        {
          CreateOrReplaceContentTypeResult result = await _contentTypeService.CreateOrReplaceAsync(payload, payload.Id, version: null, cancellationToken);
          ContentType contentType = result.ContentType ?? throw new InvalidOperationException($"'ContentTypeService.CreateOrReplaceAsync' returned null for content type 'Id={payload.Id}'.");
          _logger.LogInformation("The content type '{ContentType}' was {Action}.", contentType.DisplayName ?? contentType.UniqueName, result.Created ? "created" : "replaced");
        }
      }
    }
  }
}
