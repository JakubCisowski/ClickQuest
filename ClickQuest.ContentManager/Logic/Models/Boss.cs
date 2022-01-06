using System.Collections.Generic;
using System.Linq;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Models.Interfaces;

namespace ClickQuest.ContentManager.Logic.Models;

public class BossLootPattern
{
	public RewardType BossLootType { get; set; }
	public int BossLootId { get; set; }
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

public class Boss : IIdentifiable
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Health { get; set; }
	public string Image { get; set; }
	public string Description { get; set; }
	public List<BossLootPattern> BossLootPatterns { get; set; }
	public List<Affix> Affixes { get; set; }
}