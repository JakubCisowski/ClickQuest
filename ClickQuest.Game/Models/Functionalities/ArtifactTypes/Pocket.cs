using System.Collections.Generic;
using System.Linq;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

// On kill, chance to gain a random Dungeon Key. The chance is calculated independently of regular Dungeon Key drops.
public class Pocket : ArtifactTypeFunctionality
{
	private const double DungeonKeyRarity0Chance = 0.005;
	private const double DungeonKeyRarity1Chance = 0.004;
	private const double DungeonKeyRarity2Chance = 0.003;
	private const double DungeonKeyRarity3Chance = 0.002;
	private const double DungeonKeyRarity4Chance = 0.001;
	private const double DungeonKeyRarity5Chance = 0.0005;

	private List<double> _dungeonKeyRarityChances;
	
	public override void OnKill()
	{
		var position = CollectionsHelper.RandomizeFrequenciesListPosition(_dungeonKeyRarityChances);
		
		if (position != 0)
		{
			var dungeonKey = User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)(position - 1));
			dungeonKey.AddItem();

			// [PRERELEASE] Display dungeon key drop.
			// (GameAssets.CurrentPage as RegionPage).TestRewardsBlock.Text += $". You've got a {(Rarity) (position - 1)} Dungeon Key!";
		}
	}

	public Pocket()
	{
		ArtifactType = ArtifactType.Pocket;
		Description = "On kill, chance to gain a random Dungeon Key. The chance is calculated independently of regular Dungeon Key drops.";

		_dungeonKeyRarityChances = new List<double>()
		{
			DungeonKeyRarity0Chance, DungeonKeyRarity1Chance, DungeonKeyRarity2Chance, DungeonKeyRarity3Chance, DungeonKeyRarity4Chance, DungeonKeyRarity5Chance
		};
	}
}