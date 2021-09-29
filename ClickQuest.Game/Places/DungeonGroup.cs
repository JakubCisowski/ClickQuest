using System.Collections.Generic;
using ClickQuest.Game.Items;

namespace ClickQuest.Game.Places
{
	public class DungeonGroup
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public List<Rarity> KeyRequirementRarities { get; set; }
		public string Color { get; set; }
	}
}