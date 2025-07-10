export type CreatePokemonPayload = {
  id?: string;
  form: string;
  uniqueName: string;
  nickname?: string;
  gender?: PokemonGender;
};
//   public PokemonType? TeraType { get; set; }
//   public PokemonSize? Size { get; set; }
//   public AbilitySlot? AbilitySlot { get; set; }
//   public string? Nature { get; set; }
//   public int Experience { get; set; }
//   public IndividualValuesModel? IndividualValues { get; set; }
//   public EffortValuesModel? EffortValues { get; set; }
//   public int Vitality { get; set; }
//   public int Stamina { get; set; }
//   public byte? Friendship { get; set; }
//   public string? HeldItem { get; set; }
//   public List<string> Moves { get; set; } = [];
//   public string? Sprite { get; set; }
//   public string? Url { get; set; }
//   public string? Notes { get; set; }

export type PokemonGender = "Female" | "Male";
