using ClickQuest.Account;
using ClickQuest.Enemies;
using ClickQuest.Heroes;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
		private DispatcherTimer _poisonTimer;
		private int _poisonTicks;
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

			// Setup Timer to tick every 0.5s.
			_poisonTimer = new DispatcherTimer();
			_poisonTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			_poisonTimer.Tick += PoisonTimer_Tick;

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
				Account.User.Instance.CurrentHero.AddItem(_monster.Loot[i].Item);
				_regionPage.EquipmentFrame.Refresh();
			}

			// Display exp and loot for testing purposes.
			_regionPage.TestRewardsBlock.Text = "Loot: " + _monster.Loot[i].Item.Name + ", Exp: " + Experience.CalculateMonsterXpReward(_monster.Health);

			// Check if hero got dungeon key.
			CheckForDungeonKeyDrop();

			// Increase killing specialization amount (it will update buffs in setter).
			Account.User.Instance.CurrentHero.Specialization.SpecKillingAmount++;

			_regionPage.StatsFrame.Refresh();
		}

		private void CheckIfMonsterDied()
		{
			// If monster died - grant rewards and spawn another one.
			if (Monster.CurrentHealth <= 0)
			{
				// Reset poison.
				_poisonTimer.Stop();
				_poisonTicks = 0;

				// Grant exp and loot.
				GrantVictoryBonuses();

				// Spawn new monster.
				SpawnMonster();
			}
		}

		private void CheckForDungeonKeyDrop()
		{
			var DungeonKeyRarityChances = new List<double>();
			double EmptyLootChance = 1;
			double DungeonKeyRarity0Chance = 0;
			double DungeonKeyRarity1Chance = 0;
			double DungeonKeyRarity2Chance = 0;
			double DungeonKeyRarity3Chance = 0;
			double DungeonKeyRarity4Chance = 0;
			double DungeonKeyRarity5Chance = 0;

			// Set dungeon key drop rates.
			if (Monster.Health < 100)
			{
				EmptyLootChance = 0.05000;
				DungeonKeyRarity0Chance = 0.8000;
				DungeonKeyRarity1Chance = 0.1500;
			}
			else if (Monster.Health < 200)
			{
				EmptyLootChance = 0.05000;
				DungeonKeyRarity0Chance = 0.4500;
				DungeonKeyRarity1Chance = 0.5000;
			}

			// Set drop rates in list for randomizing algorithm.
			// Note that index 0 is for empty loot, rarities start with index 1 up to 6.
			DungeonKeyRarityChances.Add(EmptyLootChance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity0Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity1Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity2Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity3Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity4Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity5Chance);

			// Check if hero got dungeon key.
			// Randomize loot.
			double num = _rng.Next(1, 10001) / 10000d;
			int i = 0;
			while (num > DungeonKeyRarityChances[i])
			{
				num -= DungeonKeyRarityChances[i];
				i++;
			}
			// Grant dungeon key after if algorithm didn't roll empty loot.
			if (i != 0)
			{
				// Add new key to account.
				Account.User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)(i - 1)).Quantity++;

				// Display dungeon key drop.
				_regionPage.TestRewardsBlock.Text += $". You've got a {(Rarity)(i - 1)} Dungeon Key!";
			}
		}

		#region Events
		private void MonsterButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if any quest is currently assigned to this hero (if so, hero can't fight).
			if (Account.User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
			{
				// Reset poison ticks.
				_poisonTicks = 0;
				_poisonTimer.Start();

				// Calculate damage dealt to monster.
				int damage = Account.User.Instance.CurrentHero.ClickDamage;
				// Calculate crit.
				double num = _rng.Next(1, 101) / 100d;
				if (num <= Account.User.Instance.CurrentHero.CritChance)
				{
					damage *= 2;
				}
				// Apply specialization killing buff.
				damage += Account.User.Instance.CurrentHero.Specialization.SpecKillingBuff;
				// Deal damage to monster.
				Monster.CurrentHealth -= damage;

				// Check if monster is dead now.
				CheckIfMonsterDied();
			}
			else
			{
				// Display warning - you can't fight when your hero is completing quest.
				AlertBox.Show($"Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
		}

		private void PoisonTimer_Tick(object source, EventArgs e)
		{
			// When poison ends, stop poison timer.
			if (_poisonTicks >= 5)
			{
				_poisonTimer.Stop();
			}
			// Otherwise deal poison damage and check if monster died.
			else
			{
				int poisonDamage = Account.User.Instance.CurrentHero.PoisonDamage;
				_poisonTicks++;
				Monster.CurrentHealth -= poisonDamage;
				// Check if monster is dead now.
				CheckIfMonsterDied();
			}
		}
		#endregion
	}
}