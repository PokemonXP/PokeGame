﻿namespace PokeGame.Core.Pokemon;

public interface IPokemonRepository
{
  Task<Specimen?> LoadAsync(PokemonId id, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<Specimen>> LoadAsync(IEnumerable<PokemonId> ids, CancellationToken cancellationToken = default);

  Task SaveAsync(Specimen pokemon, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Specimen> specimens, CancellationToken cancellationToken = default);
}
