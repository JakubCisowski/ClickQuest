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

		public Monster() : base()
		{

		}

		public override Monster CopyEnemy()
		{
			var copy = new Monster();

			copy.Id = Id;
			copy.Name = Name;
			copy.Health = Health;
			copy.CurrentHealth = Health;
			copy.Description = Description;
			copy.CurrentHealthProgress = CurrentHealthProgress;
			copy.Loot = Loot;

			return copy;
		}
	}
}