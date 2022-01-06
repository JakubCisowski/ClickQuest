using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.UserInterface.Controls;

public partial class MonsterButton : UserControl, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	private readonly RegionPage _regionPage;

	public Monster Monster { get; set; }

	public Region Region => _regionPage.Region;

	public MonsterButton(RegionPage regionPage)
	{
		InitializeComponent();

		_regionPage = regionPage;

		CombatTimersHelper.SetupPoisonTimer();
		CombatTimersHelper.SetupAuraTimer();
		SpawnMonster();
	}

	public void SpawnMonster()
	{
		var frequencyList = Region.MonsterSpawnPatterns.Select(x => x.Frequency).ToList();
		var position = CollectionsHelper.RandomizeFrequenciesListPosition(frequencyList);
		Monster = Region.MonsterSpawnPatterns[position].Monster.CopyEnemy();

		DataContext = Monster;

		// Set Button's border based on it's spawn rate.
		MainBorder.BorderBrush = ColorsHelper.GetMonsterSpawnRarityColor(Region.MonsterSpawnPatterns.FirstOrDefault(x => x.MonsterId == Monster.Id));

		CombatTimersHelper.StartAuraTimerOnCurrentRegion();
	}

	private void MonsterButton_Click(object sender, RoutedEventArgs e)
	{
		var isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);

		if (isNoQuestActive)
		{
			CombatHelper.HandleUserClickOnEnemy();
		}
		else
		{
			AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
		}
	}
}