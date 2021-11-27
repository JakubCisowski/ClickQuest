using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items.Types;

namespace ClickQuest.Game.Core.Items.Patterns
{
	public class BossLootPattern
	{
		public int BossLootId { get; set; }
		public RewardType BossLootType { get; set; }
		public List<double> Frequencies { get; set; }

		public Item Item
		{
			get
			{
				Item item = null;

				switch (BossLootType)
				{
					case RewardType.Material:
						item = GameAssets.Materials.FirstOrDefault(x => x.Id == BossLootId);
						break;

					case RewardType.Recipe:
						item = GameAssets.Recipes.FirstOrDefault(x => x.Id == BossLootId);
						break;

					case RewardType.Artifact:
						item = GameAssets.Artifacts.FirstOrDefault(x => x.Id == BossLootId);
						break;
				}

				return item;
			}
		}
	}
}