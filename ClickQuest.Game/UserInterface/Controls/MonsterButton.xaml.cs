using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.UserInterface.Controls
{
	public partial class MonsterButton : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly RegionPage _regionPage;

		public Monster Monster { get; set; }

		public Region Region
		{
			get
			{
				return _regionPage.Region;
			}
		}

		public MonsterButton(RegionPage regionPage)
		{
			InitializeComponent();

			_regionPage = regionPage;

			CombatTimerController.SetupPoisonTimer();
			CombatTimerController.SetupAuraTimer();
			SpawnMonster();
		}

		public void SpawnMonster()
		{
			var frequencyList = Region.MonsterSpawnPatterns.Select(x => x.Frequency).ToList();
			int position = CollectionsController.RandomizeFreqenciesListPosition(frequencyList);
			Monster = Region.MonsterSpawnPatterns[position].Monster.CopyEnemy();

			DataContext = Monster;

			CombatTimerController.StartAuraTimerOnCurrentRegion();
		}

		private void MonsterButton_Click(object sender, RoutedEventArgs e)
		{
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);

			if (isNoQuestActive)
			{
				CombatController.HandleUserClickOnEnemy();

				_regionPage.StatsFrame.Refresh();
			}
			else
			{
				AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
			}
		}
	}
}