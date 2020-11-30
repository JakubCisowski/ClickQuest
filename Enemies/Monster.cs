using ClickQuest.Items;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Enemies
{
	public partial class Monster : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		#region Private Fields
		private int _id;
		private string _name;
		private int _health;
		private int _currentHealth;
		private string _image;
		private List<MonsterType> _types;
		private List<(Item Item, ItemType ItemType, double Frequency)> _loot;

		#endregion

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

		public int Health
		{
			get
			{
				return _health;
			}
			set
			{
				_health = value;
				OnPropertyChanged();
			}
		}
		public int CurrentHealth
		{
			get
			{
				return _currentHealth;
			}
			set
			{
				_currentHealth = value;
				OnPropertyChanged();
			}
		}

		public string Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
				OnPropertyChanged();
			}
		}

		public List<MonsterType> Types
		{
			get
			{
				return _types;
			}
			set
			{
				_types = value;
				OnPropertyChanged();
			}
		}

		public List<(Item Item, ItemType ItemType, double Frequency)> Loot
		{
			get
			{
				return _loot;
			}
			set
			{
				_loot = value;
				OnPropertyChanged();
			}
		}

		#endregion

		public Monster(int id, string name, int health, string image, List<MonsterType> types, List<(Item, ItemType, double)> loot)
		{
			Id = id;
			Name = name;
			Health = health;
			CurrentHealth = health;
			Types = types;
			Loot = loot;
		}

		public Monster(Monster monsterToCopy)
		{
			Id = monsterToCopy.Id;
			Name = monsterToCopy.Name;
			Health = monsterToCopy.Health;
			CurrentHealth = monsterToCopy.Health;
			Types = monsterToCopy.Types;
			Loot = monsterToCopy.Loot;
		}
	}
}