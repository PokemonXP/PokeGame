using Krakenar.Core;
using Krakenar.Infrastructure.Commands;
using MediatR;

namespace PokeGame.Seeding.Krakenar.Tasks;

internal class MigrateDatabaseTask : SeedingTask
{
  public override string? Description => "Migrates Krakenar database.";
}

internal class MigrateDatabaseTaskHandler : INotificationHandler<MigrateDatabaseTask>
{
  private readonly ICommandHandler<MigrateDatabase> _handler;

  public MigrateDatabaseTaskHandler(ICommandHandler<MigrateDatabase> handler)
  {
    _handler = handler;
  }

  public async Task Handle(MigrateDatabaseTask _, CancellationToken cancellationToken)
  {
    MigrateDatabase command = new();
    await _handler.HandleAsync(command, cancellationToken);
  }
}
