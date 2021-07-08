using ClickQuest.Player;
using ClickQuest.Items;
using ClickQuest.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Enemies
{
	public partial class Boss : Enemy
	{
		private List<(Item Item, ItemType ItemType, List<double> Frequencies)> _bossLoot;

		public List<(Item Item, ItemType ItemType, List<double> Frequencies)> BossLoot
		{
			get
			{
				return _bossLoot;
			}
			set
			{
				_bossLoot = value;
				OnPropertyChanged();
			}
		}

		// Boss constructor.
		public Boss(int id, string name, int health, string image, string description, List<(Item, ItemType, List<double>)> bossLoot) : base(id, name, health, image, description)
		{
			BossLoot = bossLoot;
		}

		public Boss(Boss bossToCopy) : base(bossToCopy)
		{
			BossLoot = bossToCopy.BossLoot;
		}
	}
}