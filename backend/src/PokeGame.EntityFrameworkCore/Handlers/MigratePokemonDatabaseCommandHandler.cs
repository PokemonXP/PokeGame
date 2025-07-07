using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Krakenar.Infrastructure.Commands;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace PokeGame.EntityFrameworkCore.Handlers;

internal class MigratePokemonDatabaseCommandHandler : MigrateDatabaseCommandHandler
{
  private readonly PokemonContext _pokemon;

  public MigratePokemonDatabaseCommandHandler(EventContext events, KrakenarContext krakenar, PokemonContext rules) : base(events, krakenar)
  {
    _pokemon = rules;
  }

  public override async Task HandleAsync(MigrateDatabase command, CancellationToken cancellationToken)
  {
    await base.HandleAsync(command, cancellationToken);

    await _pokemon.Database.MigrateAsync(cancellationToken);
  }
}
