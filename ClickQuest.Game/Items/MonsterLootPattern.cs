using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Data.GameData;

namespace ClickQuest.Game.Items
{
	public class MonsterLootPattern
	{
		public int LootId { get; set; }
		public LootType LootType { get; set; }
		public double Frequency { get; set; }

		[JsonIgnore]
		public Item Item
		{
			get
			{
				Item item = null;

				switch (LootType)
				{
					case LootType.Material:
						item = GameData.Materials.FirstOrDefault(x => x.Id == LootId);
						break;

					case LootType.Recipe:
						item = GameData.Recipes.FirstOrDefault(x => x.Id == LootId);
						break;

					case LootType.Artifact:
						item = GameData.Artifacts.FirstOrDefault(x => x.Id == LootId);
						break;
				}

				return item;
			}
		}
	}
}