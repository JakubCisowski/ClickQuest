using ClickQuest.Account;
using ClickQuest.Enemies;
using ClickQuest.Heroes;
using ClickQuest.Pages;
using ClickQuest.Places;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Controls
{
	public partial class MonsterButton : UserControl, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Private Fields
		private Monster _monster;
		private Region _region;
		private Random _rng = new Random();
		private RegionPage _regionPage;
		#endregion

		#region Properties
		public Monster Monster
		{
			get
			{
				return _monster;
			}
			set
			{
				_monster = value;
				OnPropertyChanged();
			}
		}
		#endregion

		public MonsterButton(Region region, RegionPage regionPage)
		{
			InitializeComponent();

			// Set Region and RegionPage to which the monster belongs.
			_region = region;
			_regionPage = regionPage;

			SpawnMonster();
		}

		public void SpawnMonster()
		{
			// Randomize new monster to spawn.
			double num = _rng.Next(1, 10001) / 10000d;
			int i = 0;
			while (num > _region.Monsters[i].Frequency)
			{
				num -= _region.Monsters[i].Frequency;
				i++;
			}

			// Spawn selected monster.
			Monster = new Monster(_region.Monsters[i].Monster);
			this.DataContext = Monster;
		}

		public void GrantVictoryBonuses()
		{
			// Grant experience based on moster hp.
			User.Instance.CurrentHero.Experience += Experience.CalculateMonsterXpReward(_monster.Health);

			// Randomize loot.
			double num = _rng.Next(1, 10001) / 10000d;
			int i = 0;
			while (num > _monster.Loot[i].Frequency)
			{
				num -= _monster.Loot[i].Frequency;
				i++;
			}
			// Grant loot after checking if it's not empty.
			if (_monster.Loot[i].Item.Id != 0)
			{
				Account.User.Instance.AddItem(_monster.Loot[i].Item);
				EquipmentWindow.Instance.UpdateEquipment();
			}

			// Display exp and loot for testing purposes.
			_regionPage.TestRewardsBlock.Text = "Loot: " + _monster.Loot[i].Item.Name + ", Exp: " + Experience.CalculateMonsterXpReward(_monster.Health);
		}

		#region Events
		private void MonsterButton_Click(object sender, RoutedEventArgs e)
		{
			// Deal damage to monster.
			int damage = Account.User.Instance.CurrentHero.ClickDamage;
			double num = _rng.Next(1, 101) / 100d;
			if (num <= Account.User.Instance.CurrentHero.CritChance)
			{
				damage *= 2;
			}
			Monster.CurrentHealth -= damage;

			// If monster died - grant rewards and spawn another one.
			if (Monster.CurrentHealth <= 0)
			{
				GrantVictoryBonuses();
				SpawnMonster();
			}
		}
		#endregion
	}
}