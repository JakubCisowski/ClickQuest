using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Enemies;
using ClickQuest.Items;
using ClickQuest.Places;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Extensions.InterfaceManager;

namespace ClickQuest.Pages
{
	public partial class DungeonSelectPage : Page
	{
		#region Private Fields
		private DungeonGroup _dungeonGroupSelected;
		private Dungeon _dungeonSelected;
		private Boss _bossSelected;

		#endregion

		public DungeonSelectPage()
		{
			InitializeComponent();

			ResetAndLoadDungeonGroupSelectionInterface();
		}

		public void ResetAndLoadDungeonGroupSelectionInterface()
		{
			ResetSelection();
			
			DungeonSelectPanel.Children.Clear();

			UndoButton.Click -= UndoButtonGroup_Click;
			UndoButton.Click -= UndoButtonDungeon_Click;

			var grid = new Grid();

			var col1 = new ColumnDefinition()
			{
				Width = new GridLength(1, GridUnitType.Star)
			};
			var col2 = new ColumnDefinition()
			{
				Width = new GridLength(1, GridUnitType.Star)
			};

			var row1 = new RowDefinition()
			{
				Height = new GridLength(1, GridUnitType.Star)
			};
			var row2 = new RowDefinition()
			{
				Height = new GridLength(1, GridUnitType.Star)
			};
			var row3 = new RowDefinition()
			{
				Height = new GridLength(1, GridUnitType.Star)
			};

			grid.ColumnDefinitions.Add(col1);
			grid.ColumnDefinitions.Add(col2);

			grid.RowDefinitions.Add(row1);
			grid.RowDefinitions.Add(row2);
			grid.RowDefinitions.Add(row3);

			// Create buttons for selecting dungeon groups.
			for (int i = 0; i < GameData.DungeonGroups.Count; i++)
			{
				var button = new Button()
				{
					Name = "DungeonGroup" + GameData.DungeonGroups[i].Id.ToString(),
					Width = 250,
					Height = 100
				};

				var panel = new StackPanel()
				{
					VerticalAlignment = VerticalAlignment.Center
				};

				var block = new TextBlock()
				{
					FontSize = 22,
					Text = GameData.DungeonGroups[i].Name,
					TextAlignment=TextAlignment.Center
				};

				var border = new Border()
				{
					BorderThickness = new Thickness(3),
					BorderBrush = Styles.Colors.GetRarityColor((Rarity)i),
					Width = 240,
					Height=90
				};

				var block2 = new TextBlock()
				{
					FontSize = 16,
					FontStyle=FontStyles.Italic,
					Text = GameData.DungeonGroups[i].Description,
					TextAlignment=TextAlignment.Center
				};

				border.Child = panel;

				panel.Children.Add(block);
				panel.Children.Add(block2);
				button.Content = border;

				button.Tag = GameData.DungeonGroups[i];

				button.Click += DungeonGroupButton_Click;

				grid.Children.Add(button);
				Grid.SetColumn(button, i / 3);
				Grid.SetRow(button, i % 3);
			}
			DungeonSelectPanel.Children.Add(grid);
		}

		private void ResetSelection()
		{
			_bossSelected = null;
			_dungeonGroupSelected = null;
			_dungeonSelected = null;
			UndoButton.Visibility = Visibility.Hidden;
		}

		public void LoadDungeonSelectionInterface()
		{
			DungeonSelectPanel.Children.Clear();

			UndoButton.Visibility = Visibility.Visible;

			UndoButton.Click += UndoButtonGroup_Click;

			var dungeonsOfThisGroup = GameData.Dungeons.Where(x => x.DungeonGroup == _dungeonGroupSelected).ToList();
			
			// Create buttons for selecting dungeon groups.
			for (int i = 0; i < dungeonsOfThisGroup.Count; i++)
			{
				var button = new Button()
				{
					Name = "Dungeon" + dungeonsOfThisGroup[i].Id.ToString(),
					Width = 250,
					Height = 80
				};

				var panel = new StackPanel();

				var block = new TextBlock()
				{
					FontSize = 22,
					Text = dungeonsOfThisGroup[i].Name,
					TextAlignment=TextAlignment.Center
				};

				var border = new Border()
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = new SolidColorBrush(Colors.LightGray),
					Margin=new Thickness(0,5,0,0)
				};

				var block2 = new TextBlock()
				{
					FontSize = 16,
					FontStyle=FontStyles.Italic,
					Text =  dungeonsOfThisGroup[i].Description,
					TextAlignment=TextAlignment.Center
				};

				border.Child = block2;

				panel.Children.Add(block);
				panel.Children.Add(border);

				button.Content = panel;

				button.Tag = dungeonsOfThisGroup[i];

				button.Click += DungeonButton_Click;

				DungeonSelectPanel.Children.Add(button);
			}
		}

		public void LoadBossSelectionInterface()
		{
			DungeonSelectPanel.Children.Clear();

			UndoButton.Click -= UndoButtonGroup_Click;
			UndoButton.Click += UndoButtonDungeon_Click;

			// Create buttons for selecting dungeon groups.
			for (int i = 0; i < _dungeonSelected.BossIds.Count; i++)
			{
				var boss = GameData.Bosses.FirstOrDefault(x=>x.Id == _dungeonSelected.BossIds[i]);

				var button = new Button()
				{
					Name = "Boss" + boss.Id.ToString(),
					Width = 250,
					Height = 80
				};

				var panel = new StackPanel();

				var block = new TextBlock()
				{
					FontSize = 22,
					Text = boss.Name,
					TextAlignment=TextAlignment.Center
				};

				var border = new Border()
				{
					BorderThickness = new Thickness(0.5),
					BorderBrush = new SolidColorBrush(Colors.LightGray),
					Margin=new Thickness(0,5,0,0)
				};

				var block2 = new TextBlock()
				{
					FontSize = 16,
					FontStyle=FontStyles.Italic,
					Text =  boss.Description,
					TextAlignment=TextAlignment.Center
				};

				border.Child = block2;

				panel.Children.Add(block);
				panel.Children.Add(border);

				button.Content = panel;

				button.Tag = boss;

				button.Click += BossButton_Click;

				DungeonSelectPanel.Children.Add(button);
			}
		}

		#region Events
		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(GameData.Pages["Town"]);
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Town";
			
			InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(GameData.Pages["Town"]);

			// Reset selection.
			ResetAndLoadDungeonGroupSelectionInterface();
		}
		
		private void DungeonGroupButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if any quest is currently assigned to this hero (if so, hero can't enter the dungeon).
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime)))
			{
				_dungeonGroupSelected = GetDungeonGroup(sender as Button);
				LoadDungeonSelectionInterface();

				(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon";
			}
		}

		private DungeonGroup GetDungeonGroup(Button dungeonGroupButton)
		{
			return dungeonGroupButton.Tag as DungeonGroup;
		}

		private void DungeonButton_Click(object sender, RoutedEventArgs e)
		{
			_dungeonSelected = GetDungeon(sender as Button);
			LoadBossSelectionInterface();

			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting boss";
		}
		
		private Dungeon GetDungeon(Button dungeonButton)
		{
			return dungeonButton.Tag as Dungeon;
		}
		
		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			_bossSelected = GetBoss(sender as Button);

			if(!CheckAndRemoveDungeonKeys())
			{
				AlertBox.Show($"Not enough dungeon keys to enter.\nTry to get them by completing quests and killing monsters!", MessageBoxButton.OK);
				return;
			}

			SetupBossFight();

			ResetAndLoadDungeonGroupSelectionInterface();
		}

		private bool CheckAndRemoveDungeonKeys()
		{
			// Check if user has enough dungoen keys to enter boss fight.
			var counts = _dungeonGroupSelected.KeyRequirementRarities.GroupBy(x => x).ToDictionary(k => k.Key, v => v.Count());

			foreach (var pair in counts)
			{
				if (User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)pair.Key).Quantity < pair.Value)
				{
					return false;
				}
			}

			// Remove dungeon keys from Player.
			foreach (var pair in counts)
			{
				User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)pair.Key).Quantity -= pair.Value;
			}

			return true;
		}

		private void SetupBossFight()
		{
			(GameData.Pages["DungeonBoss"] as DungeonBossPage).StartBossFight(_bossSelected.CopyEnemy());

			InterfaceController.RefreshStatsAndEquipmentPanelsOnPage(GameData.Pages["DungeonBoss"]);

			(Window.GetWindow(this) as GameWindow).LocationInfo = "Boss fight";
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(GameData.Pages["DungeonBoss"]);
		}

		private Boss GetBoss(Button bossButton)
		{
			return bossButton.Tag as Boss;
		}

		private void UndoButtonGroup_Click(object sender, RoutedEventArgs e)
		{
			_dungeonGroupSelected = null;
			ResetAndLoadDungeonGroupSelectionInterface();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon group";
		}

		private void UndoButtonDungeon_Click(object sender, RoutedEventArgs e)
		{
			_dungeonSelected = null;
			LoadDungeonSelectionInterface();
			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon";
		}

		#endregion Events
	}
}