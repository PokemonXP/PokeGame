﻿using Krakenar.Contracts;
using Krakenar.Core;
using Logitar;
using PokeGame.Core.Regions;

namespace PokeGame.Core;

public class UniqueNameAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique name is already used.";

  public string EntityType
  {
    get => (string)Data[nameof(EntityType)]!;
    private set => Data[nameof(EntityType)] = value;
  }
  public Guid EntityId
  {
    get => (Guid)Data[nameof(EntityId)]!;
    private set => Data[nameof(EntityId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(EntityType)] = EntityType;
      error.Data[nameof(EntityId)] = EntityId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueName)] = UniqueName;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueNameAlreadyUsedException(Region region, RegionId conflictId)
    : this("Region", region.Id.ToGuid(), conflictId.ToGuid(), region.UniqueName, nameof(region.UniqueName))
  {
  }
  private UniqueNameAlreadyUsedException(string entityType, Guid entityId, Guid conflictId, UniqueName uniqueName, string propertyName)
    : base(BuildMessage(entityType, entityId, conflictId, uniqueName, propertyName))
  {
    EntityType = entityType;
    EntityId = entityId;
    ConflictId = conflictId;
    UniqueName = uniqueName.Value;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string entityType, Guid entityId, Guid conflictId, UniqueName uniqueName, string propertyName)
  {
    return new ErrorMessageBuilder(ErrorMessage)
      .AddData(nameof(EntityType), entityType)
      .AddData(nameof(EntityId), entityId)
      .AddData(nameof(ConflictId), conflictId)
      .AddData(nameof(UniqueName), uniqueName)
      .AddData(nameof(PropertyName), propertyName)
      .Build();
  }
}
