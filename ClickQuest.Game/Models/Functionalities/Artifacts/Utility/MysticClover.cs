using System.Collections.Generic;
using System.Linq;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// While on a region, you have an increased chance to find rare Monsters.
// The chance to find Monsters that have a spawn chance of 5% or less is doubled, while the chance to find the most common Monsters is reduced appropriately.
public class MysticClover : ArtifactFunctionality
{
	private const double MonsterFrequencyModifier = 2;
	private const double MonsterFrequencyThreshold = 0.05;

	private List<MonsterSpawnPattern> _oldPattern;

	public override void OnRegionEnter()
	{
		var region = GameAssets.CurrentPage as RegionPage;

		_oldPattern = region.Region.MonsterSpawnPatterns;

		double totalFrequencyAdded = 0;

		foreach (var monsterSpawnPattern in region.Region.MonsterSpawnPatterns.OrderBy(x => x.Frequency))
		{
			if (monsterSpawnPattern.Frequency <= MonsterFrequencyThreshold)
			{
				totalFrequencyAdded += monsterSpawnPattern.Frequency;
				monsterSpawnPattern.Frequency *= MonsterFrequencyModifier;
			}
		}

		region.Region.MonsterSpawnPatterns.OrderBy(x => x.Frequency).Last().Frequency -= totalFrequencyAdded;
	}

	public override void OnRegionLeave()
	{
		var region = GameAssets.CurrentPage as RegionPage;

		region.Region.MonsterSpawnPatterns = _oldPattern;
		_oldPattern = default;
	}

	public MysticClover()
	{
		Name = "Mystic Clover";
	}
}