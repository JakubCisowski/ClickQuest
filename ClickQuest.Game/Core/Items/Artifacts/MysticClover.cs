using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.UserInterface.Pages;
using System.Collections.Generic;
using System.Linq;

namespace ClickQuest.Game.Core.Items.Artifacts
{
    // While on a region, you have an increased chance to find rare Monsters.
    // The chance to find Monsters that have a spawn chance of 2% or less is doubled, while the chance to find the most common Monsters is reduced appropriately.
    public class MysticClover : ArtifactFunctionality
    {
        private const double MonsterFrequencyModifier = 2;

        private List<MonsterSpawnPattern> _oldPattern;

        public override void OnRegionEnter()
        {
            RegionPage region = GameAssets.CurrentPage as RegionPage;

            _oldPattern = region.Region.MonsterSpawnPatterns;

            double totalFrequencyAdded = 0;

            foreach (MonsterSpawnPattern monsterSpawnPattern in region.Region.MonsterSpawnPatterns.OrderBy(x => x.Frequency))
            {
                if (monsterSpawnPattern.Frequency <= 0.02)
                {
                    totalFrequencyAdded += monsterSpawnPattern.Frequency;
                    monsterSpawnPattern.Frequency *= MonsterFrequencyModifier;
                }
            }

            region.Region.MonsterSpawnPatterns.OrderBy(x => x.Frequency).Last().Frequency -= totalFrequencyAdded;
        }

        public override void OnRegionLeave()
        {
            RegionPage region = GameAssets.CurrentPage as RegionPage;

            region.Region.MonsterSpawnPatterns = _oldPattern;
            _oldPattern = default;
        }

        public MysticClover()
        {
            Name = "Mystic Clover";
        }
    }
}