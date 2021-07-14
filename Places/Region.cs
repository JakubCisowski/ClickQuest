using ClickQuest.Enemies;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClickQuest.Interfaces;

namespace ClickQuest.Places
{
	public class Region : INotifyPropertyChanged, IIdentifiable
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
		private string _name;
		private string _description;
		private string _background;
		private List<MonsterSpawnPattern> _monsters;
		private int _levelRequirement;

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

		public List<MonsterSpawnPattern> Monsters
		{
			get
			{
				return _monsters;
			}
			set
			{
				_monsters = value;
				OnPropertyChanged();
			}
		}

		public int LevelRequirement
		{
			get
			{
				return _levelRequirement;
			}
			set
			{
				_levelRequirement = value;
				OnPropertyChanged();
			}
		}

		#endregion Properties

		public Region()
		{

		}
	}
}