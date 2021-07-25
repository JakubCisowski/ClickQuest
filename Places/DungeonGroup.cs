using System.Collections.Generic;

namespace ClickQuest.Places
{
	public class DungeonGroup
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public List<int> KeyRequirementRarities { get; set; }

		public string Color { get; set; }
	}
}