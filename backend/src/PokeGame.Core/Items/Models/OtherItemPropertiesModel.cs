﻿using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Models;

public record OtherItemPropertiesModel : IOtherItemProperties
{
  [JsonConstructor]
  public OtherItemPropertiesModel()
  {
  }

  public OtherItemPropertiesModel(IOtherItemProperties _) : this()
  {
  }
}
