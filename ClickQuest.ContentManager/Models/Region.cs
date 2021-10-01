using System.Collections.Generic;

namespace ClickQuest.ContentManager.Models
{
	public class MonsterSpawnPattern
	{
		public int MonsterId { get; set; }
		public double Frequency { get; set; }
	}
	
	public class Region
	{
		public int Id { get; set; }
		public int LevelRequirement { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Background { get; set; }
		public List<MonsterSpawnPattern> Monsters { get; set; }

		public Region()
		{
			
		}
	}
}