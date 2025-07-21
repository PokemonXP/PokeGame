using Logitar.EventSourcing;
using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Events;

public record MaterialPropertiesChanged(MaterialProperties Properties) : DomainEvent;
