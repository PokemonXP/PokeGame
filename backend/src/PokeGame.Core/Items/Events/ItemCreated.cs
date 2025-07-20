using Krakenar.Core;
using Logitar.EventSourcing;

namespace PokeGame.Core.Items.Events;

public record ItemCreated(ItemCategory Category, UniqueName UniqueName, Price? Price) : DomainEvent;
