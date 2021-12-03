using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ClickQuest.Game.Core.Places
{
    public class Dungeon : INotifyPropertyChanged, IIdentifiable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public int DungeonGroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Background { get; set; }
        public List<int> BossIds { get; set; }

        public DungeonGroup DungeonGroup
        {
            get
            {
                return GameAssets.DungeonGroups.FirstOrDefault(x => x.Id == DungeonGroupId);
            }
        }
    }
}