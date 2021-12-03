using System.Collections.Generic;
using System.Linq;

namespace ClickQuest.ContentManager.GameData.Models
{
    public class MonsterSpawnPattern
    {
        public int MonsterId { get; set; }
        public double Frequency { get; set; }

        public Monster Monster
        {
            get
            {
                return GameContent.Monsters.FirstOrDefault(x => x.Id == MonsterId);
            }
        }
    }

    public class Region : IIdentifiable
    {
        public int Id { get; set; }
        public int LevelRequirement { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Background { get; set; }
        public List<MonsterSpawnPattern> MonsterSpawnPatterns { get; set; }
    }
}