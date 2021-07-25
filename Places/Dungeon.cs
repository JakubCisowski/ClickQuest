using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ClickQuest.Data;
using ClickQuest.Interfaces;

namespace ClickQuest.Places
{
	public class Dungeon : INotifyPropertyChanged, IIdentifiable
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields

		private int _id;
		private int _dungeonGroupId;
		private string _name;
		private string _background;
		private string _description;
		private List<int> _bossIds;

		#endregion Private Fields

		#region Properties

		public int Id
		{
			get { return _id; }
			set
			{
				_id = value;
				OnPropertyChanged();
			}
		}

		public int DungeonGroupId
		{
			get { return _dungeonGroupId; }
			set
			{
				_dungeonGroupId = value;
				OnPropertyChanged();
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged();
			}
		}

		public string Background
		{
			get { return _background; }
			set
			{
				_background = value;
				OnPropertyChanged();
			}
		}

		public List<int> BossIds
		{
			get { return _bossIds; }
			set
			{
				_bossIds = value;
				OnPropertyChanged();
			}
		}

		public DungeonGroup DungeonGroup
		{
			get { return GameData.DungeonGroups.FirstOrDefault(x => x.Id == DungeonGroupId); }
		}

		#endregion Properties
	}
}