using System.Collections.Generic;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class Quest
	{
		public int Id { get; set; }
		public bool Rare { get; set; }
		public string HeroClass { get; set; }
		public string Name { get; set; }
		public int Duration { get; set; }
		public string Description { get; set; }
		public List<int> RewardRecipeIds { get; set; }
		public List<int> RewardBlessingIds { get; set; }
		public List<int> RewardMaterialIds { get; set; }
		public List<int> RewardIngotIds { get; set; }

		public Quest()
		{
			
		}
	}
}