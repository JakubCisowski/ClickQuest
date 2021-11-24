using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class MonsterLootPattern
	{
		public RewardType MonsterLootType { get; set; }
		public int MonsterLootId { get; set; }
		public double Frequency { get; set; }
		public bool BestiaryDiscovered{ get; set; }

		public Item Item
		{
			get
			{
				Item item = null;

				switch (MonsterLootType)
				{
					case RewardType.Material:
						item = GameContent.Materials.FirstOrDefault(x => x.Id == MonsterLootId);
						break;

					case RewardType.Recipe:
						item = GameContent.Recipes.FirstOrDefault(x => x.Id == MonsterLootId);
						break;

					case RewardType.Artifact:
						item = GameContent.Artifacts.FirstOrDefault(x => x.Id == MonsterLootId);
						break;
				}

				return item;
			}
		}
	}

	public class Monster : IIdentifiable
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Health { get; set; }
		public string Image { get; set; }
		public bool BestiaryDiscovered{ get; set; }
		public List<MonsterLootPattern> MonsterLootPatterns { get; set; }
	}
}