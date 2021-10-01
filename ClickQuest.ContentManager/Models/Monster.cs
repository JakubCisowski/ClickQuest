using System.Collections.Generic;

namespace ClickQuest.ContentManager.Models
{
	public class Loot
	{
		public LootType LootType { get; set; }
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
		public List<Loot> Loot { get; set; }

		public Monster()
		{
			
		}
	}
}