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
			get
			{
				return _id;
			}
			set
			{
				_id = value;
				
			}
		}

		public int DungeonGroupId
		{
			get
			{
				return _dungeonGroupId;
			}
			set
			{
				_dungeonGroupId = value;
				
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

		public List<int> BossIds
		{
			get
			{
				return _bossIds;
			}
			set
			{
				_bossIds = value;
				
			}
		}

		public DungeonGroup DungeonGroup
		{
			get
			{
				return GameData.DungeonGroups.FirstOrDefault(x => x.Id == DungeonGroupId);
			}
		}

		#endregion Properties
	}
}