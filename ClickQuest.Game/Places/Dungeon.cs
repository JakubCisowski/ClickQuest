using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Interfaces;

namespace ClickQuest.Game.Places
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
				return GameData.DungeonGroups.FirstOrDefault(x => x.Id == DungeonGroupId);
			}
		}
	}
}