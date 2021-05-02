using ClickQuest.Account;
using ClickQuest.Items;
using ClickQuest.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Enemies
{
	public partial class Monster : Enemy
	{
		private List<(Item Item, ItemType ItemType, double Frequency)> _loot;

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

		// Common monster constructor.
		public Monster(int id, string name, int health, string image, string description, List<(Item, ItemType, double)> loot) : base(id, name, health, image, description)
		{
			Loot = loot;
		}

		public Monster(Monster monsterToCopy) : base(monsterToCopy)
		{
			Loot = monsterToCopy.Loot;
		}
	}
}