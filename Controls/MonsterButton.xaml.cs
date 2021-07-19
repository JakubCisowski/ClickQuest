using ClickQuest.Player;
using ClickQuest.Enemies;
using ClickQuest.Heroes;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using ClickQuest.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ClickQuest.Data;
using ClickQuest.Heroes.Buffs;

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
		private Random _rng = new Random();
		private RegionPage _regionPage;
		private DispatcherTimer _poisonTimer;
		private DispatcherTimer _auraTimer;
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

		public Region Region 
		{
			get
			{
				return _regionPage.Region;
			}
		}
		#endregion

		public MonsterButton(RegionPage regionPage)
		{
			InitializeComponent();

			_regionPage = regionPage;

			SetupPoisonTimer();
			SetupAuraTimer();
			SpawnMonster();
		}

		public void SpawnMonster()
		{
			var frequencyList = Region.Monsters.Select(x=>x.Frequency).ToList();
			int position = RandomizeFreqenciesListPosition(frequencyList);
			Monster = Region.Monsters[position].GetMonster().CopyEnemy();
			
			this.DataContext = Monster;
			
			Extensions.CombatManager.CombatController.StartAuraTimerOnCurrentRegion();
		}

		private int RandomizeFreqenciesListPosition(List<double> frequencies)
		{
			double randomizedValue = _rng.Next(1, 10001) / 10000d;
			int i = 0;

			while (randomizedValue > frequencies[i])
			{
				randomizedValue -= frequencies[i];
				i++;
			}

			return i;
		}

		public void GrantVictoryBonuses()
		{
			int experienceGained = Experience.CalculateMonsterXpReward(_monster.Health);
			User.Instance.CurrentHero.Experience += experienceGained;

			var frequencyList = _monster.Loot.Select(x => x.Frequency).ToList();
			int position = RandomizeFreqenciesListPosition(frequencyList);
			var selectedLoot = _monster.Loot[position].Item;
			
			if (selectedLoot.Id != 0)
			{
				selectedLoot.AddItem();
			}

			// [PRERELEASE] Display exp and loot for testing purposes.
			_regionPage.TestRewardsBlock.Text = "Loot: " + selectedLoot.Name + ", Exp: " + experienceGained;

			CheckForDungeonKeyDrop();

			_regionPage.StatsFrame.Refresh();
		}

		public void StopCombatTimers()
		{
			StopPoisonTimer();
			_auraTimer.Stop();
		}

		public void StopPoisonTimer()
		{
			_poisonTimer.Stop();
			_poisonTicks = 0;
		}

		public void StartAuraTimer()
		{
			if (User.Instance.CurrentHero != null)
			{
				_auraTimer.Interval = TimeSpan.FromSeconds(1d / User.Instance.CurrentHero.AuraAttackSpeed);

				_auraTimer.Start();
			}
		}

		private void HandleMonsterDeathIfDefeated()
		{
			if (Monster.CurrentHealth <= 0)
			{
				StopPoisonTimer();
				GrantVictoryBonuses();
				SpawnMonster();
			}
		}

		private void CheckForDungeonKeyDrop()
		{
			var DungeonKeyRarityChances = DungeonKey.CreateRarityChancesList(Monster.Health);

			var position = RandomizeFreqenciesListPosition(DungeonKeyRarityChances);
			
			// Grant dungeon key after if algorithm didn't roll empty loot.
			if (position != 0)
			{
				var dungeonKey = User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)(position - 1));
				dungeonKey.AddItem();

				// [PRERELEASE] Display dungeon key drop.
				_regionPage.TestRewardsBlock.Text += $". You've got a {(Rarity)(position - 1)} Dungeon Key!";
			}
		}

		#region Events
		private void MonsterButton_Click(object sender, RoutedEventArgs e)
		{
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime));

			if (isNoQuestActive)
			{
				StartPoisonTimer();

				int damage =  User.Instance.CurrentHero.CalculateClickDamage();
				Monster.CurrentHealth -= damage;

				User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Clicking]++;

				HandleMonsterDeathIfDefeated();

				_regionPage.StatsFrame.Refresh();
			}
			else
			{
				AlertBox.Show($"Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
		}

		

		private int CalculateAuraTickDamage()
		{
			return (int)Math.Ceiling(User.Instance.CurrentHero.AuraDamage * Monster.Health);
		}
		private void StartPoisonTimer()
		{
			if (User.Instance.CurrentHero.PoisonDamage>0)
			{
				_poisonTicks = 0;
				_poisonTimer.Start();
			}
		}

		private void PoisonTimer_Tick(object source, EventArgs e)
		{
			var poisonTicksMax = 5;
			
			if (_poisonTicks >= poisonTicksMax)
			{
				_poisonTimer.Stop();
			}
			else
			{
				int poisonDamage = User.Instance.CurrentHero.PoisonDamage;
				Monster.CurrentHealth -= poisonDamage;

				_poisonTicks++;

				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.PoisonTicksAmount, 1);

				HandleMonsterDeathIfDefeated();
			}
		}

		private void AuraTimer_Tick(object source, EventArgs e)
		{
			if(User.Instance.CurrentHero != null)
			{
				int auraDamage = CalculateAuraTickDamage();
				Monster.CurrentHealth -= auraDamage;
				HandleMonsterDeathIfDefeated();
			}
		}

		private void SetupAuraTimer()
		{
			_auraTimer = new DispatcherTimer();
			_auraTimer.Tick+=AuraTimer_Tick;
		}

		private void SetupPoisonTimer()
		{
			var poisonIntervalMs = 500;
			_poisonTimer = new DispatcherTimer();
			_poisonTimer.Interval = new TimeSpan(0, 0, 0, 0, poisonIntervalMs);
			_poisonTimer.Tick += PoisonTimer_Tick;
			_poisonTicks = 0;
		}
		#endregion
	}
}	