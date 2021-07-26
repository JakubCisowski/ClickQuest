using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClickQuest.Interfaces;

namespace ClickQuest.Places
{
	public class Region : INotifyPropertyChanged, IIdentifiable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public int Id{ get; set; }

		public string Name{ get; set; }

		public string Description{ get; set; }

		public string Background{ get; set; }

		public List<MonsterSpawnPattern> Monsters{ get; set; }

		public int LevelRequirement{ get; set; }
	}
}