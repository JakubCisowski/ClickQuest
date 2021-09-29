using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Data.GameData;

namespace ClickQuest.Game.Items
{
	public class BossLootPattern
	{
		public int ItemId { get; set; }
		public ItemType ItemType { get; set; }
		public List<double> Frequencies { get; set; }

		[JsonIgnore]
		public Item Item
		{
			get
			{
				Item item = null;

				switch (ItemType)
				{
					case ItemType.Material:
						item = GameData.Materials.FirstOrDefault(x => x.Id == ItemId);
						break;

					case ItemType.Recipe:
						item = GameData.Recipes.FirstOrDefault(x => x.Id == ItemId);
						break;

					case ItemType.Artifact:
						item = GameData.Artifacts.FirstOrDefault(x => x.Id == ItemId);
						break;
				}

				return item;
			}
		}
	}
}