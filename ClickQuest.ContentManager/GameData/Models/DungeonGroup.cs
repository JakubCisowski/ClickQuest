using System.Collections.Generic;

namespace ClickQuest.ContentManager.GameData.Models
{
    public class DungeonGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public List<Rarity> KeyRequirementRarities { get; set; }
    }
}