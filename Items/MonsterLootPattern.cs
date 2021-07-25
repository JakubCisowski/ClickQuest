using System.Linq;
using ClickQuest.Data;

namespace ClickQuest.Items
{
	public class MonsterLootPattern
	{
		public int ItemId { get; set; }
		public ItemType ItemType { get; set; }
		public double Frequency { get; set; }

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