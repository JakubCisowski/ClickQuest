using ClickQuest.Game.Core.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace ClickQuest.Game.Core.Places
{
    public class Region : INotifyPropertyChanged, IIdentifiable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Background { get; set; }
        public List<MonsterSpawnPattern> MonsterSpawnPatterns { get; set; }
        public int LevelRequirement { get; set; }
    }
}