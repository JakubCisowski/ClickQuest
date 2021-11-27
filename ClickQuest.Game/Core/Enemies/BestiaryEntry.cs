using ClickQuest.Game.Core.Items.Types;

namespace ClickQuest.Game.Core.Enemies
{
	public class BestiaryEntry
	{
		public int Id{ get; set; }
		public BestiaryEntryType EntryType{ get; set; }
		public RewardType LootType{ get; set; }
	}
}