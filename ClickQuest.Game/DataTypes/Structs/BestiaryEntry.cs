using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.DataTypes.Structs;

public class BestiaryEntry
{
	public int Id { get; set; }
	public BestiaryEntryType EntryType { get; set; }
	public int RelatedEnemyId { get; set; }
	public RewardType LootType { get; set; }
}