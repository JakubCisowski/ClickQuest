using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items.Types;
using System.Linq;

namespace ClickQuest.Game.Core.Items.Patterns
{
    public class MonsterLootPattern
    {
        public int MonsterLootId { get; set; }
        public RewardType MonsterLootType { get; set; }
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
}