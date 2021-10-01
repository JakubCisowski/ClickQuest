using System.Collections.Generic;

namespace ClickQuest.ContentManager.Models
{
	public class BossLoot
	{
		public LootType LootType { get; set; }
		public int LootId { get; set; }
		public List<double> Frequencies { get; set; }

		public BossLoot()
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
		public List<BossLoot> BossLoot { get; set; }

		public Boss()
		{
			
		}
	}
}