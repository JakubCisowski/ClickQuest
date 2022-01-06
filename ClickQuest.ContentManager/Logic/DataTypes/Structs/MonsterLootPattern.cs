using System.Linq;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Models;

namespace ClickQuest.ContentManager.Logic.DataTypes.Structs;

public class MonsterLootPattern
{
	public RewardType MonsterLootType { get; set; }
	public int MonsterLootId { get; set; }
	public double Frequency { get; set; }

	public Item Item
	{
		get
		{
			Item item = null;

			switch (MonsterLootType)
			{
				case RewardType.Material:
					item = GameAssets.Materials.FirstOrDefault(x => x.Id == MonsterLootId);
					break;

				case RewardType.Recipe:
					item = GameAssets.Recipes.FirstOrDefault(x => x.Id == MonsterLootId);
					break;

				case RewardType.Artifact:
					item = GameAssets.Artifacts.FirstOrDefault(x => x.Id == MonsterLootId);
					break;
			}

			return item;
		}
	}
}