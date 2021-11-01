using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;

namespace ClickQuest.Game.Core.Items
{
	public class MonsterLootPattern
	{
		public int LootId { get; set; }
		public RewardType RewardType { get; set; }
		public double Frequency { get; set; }

		[JsonIgnore]
		public Item Item
		{
			get
			{
				Item item = null;

				switch (RewardType)
				{
					case RewardType.Material:
						item = GameAssets.Materials.FirstOrDefault(x => x.Id == LootId);
						break;

					case RewardType.Recipe:
						item = GameAssets.Recipes.FirstOrDefault(x => x.Id == LootId);
						break;

					case RewardType.Artifact:
						item = GameAssets.Artifacts.FirstOrDefault(x => x.Id == LootId);
						break;
				}

				return item;
			}
		}
	}
}