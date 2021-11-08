using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class MonsterSpawnPattern
	{
		public int MonsterId { get; set; }
		public double Frequency { get; set; }

		public Monster Monster => GameContent.Monsters.FirstOrDefault(x => x.Id == MonsterId);
	}

	public class Region : IIdentifiable
	{
		public int Id { get; set; }
		public int LevelRequirement { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Background { get; set; }
		public List<MonsterSpawnPattern> MonsterSpawnPatterns { get; set; }
	}
}