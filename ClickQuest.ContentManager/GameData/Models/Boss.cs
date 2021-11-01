using System.Collections.Generic;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class BossLootPattern
	{
		public RewardType RewardType { get; set; }
		public int LootId { get; set; }
		public List<double> Frequencies { get; set; }

		public BossLootPattern()
		{
			
		}
	}
	
	public class Boss
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Health { get; set; }
		public string Image { get; set; }
		public string Description { get; set; }
		public List<BossLootPattern> BossLootPatterns { get; set; }

		public Boss()
		{
			
		}
	}
}