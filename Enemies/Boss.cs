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
		private List<BossLootPattern> _bossLoot;

		public List<BossLootPattern> BossLoot
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

		public Boss() : base()
		{

		}

		public override Boss CopyEnemy()
		{
			var copy = new Boss();

			copy.Id = Id;
			copy.Name = Name;
			copy.Health = Health;
			copy.CurrentHealth = Health;
			copy.Description = Description;
			copy.CurrentHealthProgress = CurrentHealthProgress;
			copy.BossLoot = BossLoot;

			return copy;
		}
	}
}