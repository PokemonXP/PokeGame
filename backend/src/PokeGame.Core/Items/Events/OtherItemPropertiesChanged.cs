using Logitar.EventSourcing;
using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Events;

public record OtherItemPropertiesChanged(OtherItemProperties Properties) : DomainEvent;
