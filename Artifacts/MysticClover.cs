using System.Collections.Generic;
using System.Linq;
using ClickQuest.Data.GameData;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;

namespace ClickQuest.Artifacts
{
	public class MysticClover : ArtifactFunctionality
	{
		private const double MonsterFrequencyModifier = 2;

		private List<MonsterSpawnPattern> _oldPattern;

		public override void OnRegionEnter()
		{
			var region = GameData.CurrentPage as RegionPage;
			
			_oldPattern = region.Region.Monsters;

			double totalFrequencyAdded = 0;
			
			foreach (var monsterSpawnPattern in region.Region.Monsters.OrderBy(x=>x.Frequency))
			{
				if (monsterSpawnPattern.Frequency <= 0.02)
				{
					totalFrequencyAdded += monsterSpawnPattern.Frequency;
					monsterSpawnPattern.Frequency *= MonsterFrequencyModifier;
				}
			}

			region.Region.Monsters.OrderBy(x => x.Frequency).Last().Frequency -= totalFrequencyAdded;
		}

		public override void OnRegionLeave()
		{
			var region = GameData.CurrentPage as RegionPage;
			
			region.Region.Monsters = _oldPattern;
			_oldPattern = default;
		}
		
		public MysticClover()
		{
			Name = "Mystic Clover";
		}
	}
}