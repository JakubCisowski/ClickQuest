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
				
			}
		}

		#endregion Properties
	}
}