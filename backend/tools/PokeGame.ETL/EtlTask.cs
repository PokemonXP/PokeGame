using Logitar;
using MediatR;

namespace PokeGame.ETL;

internal abstract class EtlTask : INotification
{
  /// <summary>
  /// Gets or sets the unique identifier of the task.
  /// </summary>
  public virtual Guid Id { get; protected set; }

  /// <summary>
  /// Gets or sets the date and time when the task started.
  /// </summary>
  public virtual DateTime StartedOn { get; protected set; }
  /// <summary>
  /// Gets or sets the date and time when the task ended.
  /// </summary>
  public virtual DateTime? EndedOn { get; protected set; }
  /// <summary>
  /// Gets or sets the task execution duration. Null will be returned if the task execution has not ended.
  /// </summary>
  public virtual TimeSpan? Duration => EndedOn.HasValue ? EndedOn.Value - StartedOn : null;

  /// <summary>
  /// Gets the name of the task.
  /// </summary>
  public virtual string Name => NameOverride ?? GetType().Name;
  /// <summary>
  /// Gets or sets the task name override. Please give your task a significant name.
  /// <br />When left null, the name of the task type will be used to represent the task.
  /// </summary>
  public virtual string? NameOverride { get; protected set; }
  /// <summary>
  /// Gets or sets the description of the task. Please provide a description to help understand what this task is doing.
  /// </summary>
  public virtual string? Description { get; protected set; }

  protected EtlTask(string? description = null, string? nameOverride = null, Guid? id = null, DateTime? startedOn = null)
  {
    Id = id ?? Guid.NewGuid();

    StartedOn = startedOn ?? DateTime.Now;

    NameOverride = nameOverride?.CleanTrim();
    Description = description?.CleanTrim();
  }

  /// <summary>
  /// Marks this task as completed.
  /// </summary>
  /// <param name="on">The date and time when the task was completed.</param>
  public void Complete(DateTime? on = null)
  {
    EndedOn = on ?? DateTime.Now;
  }

  public override bool Equals(object? obj) => obj is EtlTask task && task.GetType().Equals(GetType()) && task.Id == Id;
  public override int GetHashCode() => HashCode.Combine(GetType(), Id);
  public override string ToString() => $"{Name} (Id={Id})";
}
