using ClickQuest.Player;
using ClickQuest.Items;
using ClickQuest.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Enemies
{
	public partial class Monster : Enemy
	{
		private List<MonsterLootPattern> _loot;

		public List<MonsterLootPattern> Loot
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
		public Monster(int id, string name, int health, string image, string description, List<MonsterLootPattern> loot) : base(id, name, health, image, description)
		{
			Loot = loot;
		}

		public Monster(Monster monsterToCopy) : base(monsterToCopy)
		{
			Loot = monsterToCopy.Loot;
		}

		public Monster() : base()
		{

		}
	}
}