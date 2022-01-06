using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.UserInterface.Pages;

public partial class DungeonBossPage : Page, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler PropertyChanged;

	public Boss Boss { get; set; }
	public int Duration { get; set; }

	public DungeonBossPage()
	{
		InitializeComponent();
	}

	public void StartBossFight(Boss boss)
	{
		CombatTimerController.SetupFightTimer();
		CombatTimerController.SetupPoisonTimer();

		BossButton.IsEnabled = true;
		TownButton.Visibility = Visibility.Hidden;
		BossHealthBorder.BorderBrush = (SolidColorBrush)FindResource("BrushGold");

		BindFightInfoToInterface(boss);

		// Mark the Boss as discovered.
		if (!GameAssets.BestiaryEntries.Any(x => x.EntryType == BestiaryEntryType.Boss && x.Id == boss.Id))
		{
			GameAssets.BestiaryEntries.Add(new BestiaryEntry
			{
				Id = boss.Id,
				EntryType = BestiaryEntryType.Boss
			});
		}

		CombatTimerController.BossFightTimer.Start();
	}

	private void BindFightInfoToInterface(Boss boss)
	{
		Boss = boss;
		DataContext = Boss;

		var binding = new Binding("Duration")
		{
			Source = this,
			StringFormat = "Time remaining: {0}s"
		};
		DungeonTimerBlock.SetBinding(TextBlock.TextProperty, binding);

		var affixesStringList = new List<string>();

		foreach (var affix in boss.Affixes)
		{
			var affixString = string.Concat(affix.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

			affixesStringList.Add(affixString);
		}

		BossAffixesBlock.Text = string.Join(" / ", affixesStringList);
	}

	public void HandleInterfaceAfterBossDeath()
	{
		BossButton.IsEnabled = false;
		TownButton.Visibility = Visibility.Visible;
		BossHealthBorder.BorderBrush = (SolidColorBrush)FindResource("BrushGray3");

		InterfaceController.RefreshCurrentEquipmentPanelTabOnCurrentPage();
		(StatsFrame.Content as HeroStatsPage).RefreshAllDynamicStatsAndToolTips();
	}

	private void BossButton_Click(object sender, RoutedEventArgs e)
	{
		CombatController.HandleUserClickOnEnemy();
	}

	private void TownButton_Click(object sender, RoutedEventArgs e)
	{
		InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");
	}
}