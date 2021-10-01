using System.Collections.Generic;

namespace ClickQuest.ContentManager.Models
{
	public class DungeonGroup
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }
		public string Description { get; set; }
		public List<Rarity> KeyRequirementRarities { get; set; }
		public string Colour { get; set; }

		public DungeonGroup()
		{
			
		}
	}
}