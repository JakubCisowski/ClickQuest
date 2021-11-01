using System.Collections.Generic;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class MonsterLootPattern
	{
		public RewardType RewardType { get; set; }
		public int LootId { get; set; }
		public double Frequency { get; set; }
	}
	
	public class Monster
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Health { get; set; }
		public string Image { get; set; }
		public List<MonsterLootPattern> MonsterLootPatterns { get; set; }

		public Monster()
		{
			
		}
	}
}