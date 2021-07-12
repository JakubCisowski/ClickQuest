using System.Collections.Generic;
using System.Drawing;

namespace ClickQuest.Places
{
	public partial class DungeonGroup
	{
		private int _id;
		private string _name;
		private string _description;
		private List<int> _keyRequirementRarities;
		private string _color;

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

		public List<int> KeyRequirementRarities
		{
			get
			{
				return _keyRequirementRarities;
			}
			set
			{
				_keyRequirementRarities = value;
			}
		}

		public string Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
			}
		}

		public DungeonGroup()
		{

		}
	}
}