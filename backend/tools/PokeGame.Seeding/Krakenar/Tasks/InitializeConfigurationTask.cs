using Krakenar.Core;
using Krakenar.Core.Configurations.Commands;
using MediatR;
using PokeGame.Seeding.Settings;

namespace PokeGame.Seeding.Krakenar.Tasks;

internal class InitializeConfigurationTask : SeedingTask
{
  public override string? Description => "Initializes Krakenar configuration.";
  public DefaultSettings Defaults { get; }

  public InitializeConfigurationTask(DefaultSettings defaults)
  {
    Defaults = defaults;
  }
}

internal class InitializeConfigurationTaskHandler : INotificationHandler<InitializeConfigurationTask>
{
  private readonly ICommandHandler<InitializeConfiguration> _handler;

  public InitializeConfigurationTaskHandler(ICommandHandler<InitializeConfiguration> handler)
  {
    _handler = handler;
  }

  public async Task Handle(InitializeConfigurationTask task, CancellationToken cancellationToken)
  {
    DefaultSettings defaults = task.Defaults;
    InitializeConfiguration command = new(defaults.Locale, defaults.UniqueName, defaults.Password);
    await _handler.HandleAsync(command, cancellationToken);
  }
}
