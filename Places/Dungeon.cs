using ClickQuest.Enemies;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Places
{
	public class Dungeon : INotifyPropertyChanged
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
		private DungeonGroup _dungeonGroup;
		private string _name;
		private string _background;
		private string _description;
		private List<Monster> _bosses;

		#endregion Private Fields

		#region Properties

		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
				OnPropertyChanged();
			}
		}

		public DungeonGroup DungeonGroup
		{
			get
			{
				return _dungeonGroup;
			}
			set
			{
				_dungeonGroup = value;
				OnPropertyChanged();
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
				OnPropertyChanged();
			}
		}

		public string Background
		{
			get
			{
				return _background;
			}
			set
			{
				_background = value;
				OnPropertyChanged();
			}
		}

		public List<Monster> Bosses
		{
			get
			{
				return _bosses;
			}
			set
			{
				_bosses = value;
				OnPropertyChanged();
			}
		}

		#endregion Properties

		public Dungeon(int id, DungeonGroup dungeonGroup, string name, string background, string description, List<Monster> bosses)
		{
			Id = id;
			DungeonGroup = dungeonGroup;
			Name = name;
			Background = background;
			Description = description;
			Bosses = bosses;
		}
	}
}