using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Windows;
using Colors = ClickQuest.Game.Extensions.UserInterface.ColorsController;

namespace ClickQuest.Game.UserInterface.Pages
{
	public partial class DungeonSelectPage : Page
	{
		private DungeonGroup _dungeonGroupSelected;
		private Dungeon _dungeonSelected;
		private Boss _bossSelected;

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

			Grid grid = new Grid();

			ColumnDefinition col1 = new ColumnDefinition
			{
				Width = new GridLength(1, GridUnitType.Star)
			};
			ColumnDefinition col2 = new ColumnDefinition
			{
				Width = new GridLength(1, GridUnitType.Star)
			};

			RowDefinition row1 = new RowDefinition
			{
				Height = new GridLength(1, GridUnitType.Star)
			};
			RowDefinition row2 = new RowDefinition
			{
				Height = new GridLength(1, GridUnitType.Star)
			};
			RowDefinition row3 = new RowDefinition
			{
				Height = new GridLength(1, GridUnitType.Star)
			};

			grid.ColumnDefinitions.Add(col1);
			grid.ColumnDefinitions.Add(col2);

			grid.RowDefinitions.Add(row1);
			grid.RowDefinitions.Add(row2);
			grid.RowDefinitions.Add(row3);

			// Create buttons for selecting dungeon groups.
			for (var i = 0; i < GameAssets.DungeonGroups.Count; i++)
			{
				Button button = new Button
				{
					Background = (SolidColorBrush) FindResource("BrushGray1"),
					Name = "DungeonGroup" + GameAssets.DungeonGroups[i].Id,
					Width = 250,
					Height = 100
				};

				StackPanel panel = new StackPanel
				{
					VerticalAlignment = VerticalAlignment.Center
				};

				TextBlock block = new TextBlock
				{
					FontSize = 22,
					Text = GameAssets.DungeonGroups[i].Name,
					TextAlignment = TextAlignment.Center
				};

				Border border = new Border
				{
					BorderThickness = new Thickness(3),
					BorderBrush = Colors.GetRarityColor((Rarity) i),
					Width = 240,
					Height = 90
				};

				TextBlock block2 = new TextBlock
				{
					FontSize = 16,
					FontStyle = FontStyles.Italic,
					Text = GameAssets.DungeonGroups[i].Description,
					TextAlignment = TextAlignment.Center
				};

				border.Child = panel;

				panel.Children.Add(block);
				panel.Children.Add(block2);
				button.Content = border;

				button.Tag = GameAssets.DungeonGroups[i];

				button.Click += DungeonGroupButton_Click;

				grid.Children.Add(button);
				Grid.SetColumn(button, i / 3);
				Grid.SetRow(button, i % 3);
			}

			DungeonSelectPanel.Children.Add(grid);
		}

		public void LoadDungeonSelectionInterface()
		{
			DungeonSelectPanel.Children.Clear();

			UndoButton.Visibility = Visibility.Visible;

			UndoButton.Click += UndoButtonGroup_Click;

			var dungeonsOfThisGroup = GameAssets.Dungeons.Where(x => x.DungeonGroup == _dungeonGroupSelected).ToList();

			// Create buttons for selecting dungeon groups.
			foreach (Dungeon dungeon in dungeonsOfThisGroup)
			{
				Button button = new Button
				{
					Background = (SolidColorBrush) FindResource("BrushGray1"),
					Name = "Dungeon" + dungeon.Id,
					Width = 330,
					Height = 100
				};

				StackPanel panel = new StackPanel();

				TextBlock block = new TextBlock
				{
					FontSize = 25,
					Text = dungeon.Name,
					TextAlignment = TextAlignment.Center,
					FontFamily = (FontFamily) FindResource("FontFancy")
				};

				panel.Children.Add(block);

				button.Content = panel;

				button.Tag = dungeon;

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
			foreach (int bossId in _dungeonSelected.BossIds)
			{
				var boss = GameAssets.Bosses.FirstOrDefault(x => x.Id == bossId);

				Button button = new Button
				{
					Background = (SolidColorBrush) FindResource("BrushGray1"),
					Name = "Boss" + boss.Id,
					Width = 330,
					Height = 120
				};

				Grid grid = new Grid
				{
					Width = 320,
					Height = 110
				};

				TextBlock block = new TextBlock
				{
					FontSize = 25,
					Text = boss.Name,
					TextAlignment = TextAlignment.Center,
					FontFamily = (FontFamily) FindResource("FontFancy"),
					VerticalAlignment = VerticalAlignment.Top,
					Margin = new Thickness(0, 10, 0, 0)
				};

				TextBlock block2 = new TextBlock
				{
					FontSize = 18,
					TextAlignment = TextAlignment.Center,
					FontFamily = (FontFamily) FindResource("FontFancy"),
					VerticalAlignment = VerticalAlignment.Center
				};

				block2.Inlines.Add(new Run("Health: "));
				block2.Inlines.Add(new Run(boss.MaxHealth.ToString())
				{
					Foreground = (SolidColorBrush) FindResource("BrushRed")
				});

				TextBlock block3 = new TextBlock
				{
					FontSize = 16,
					FontFamily = (FontFamily) FindResource("FontRegularItalic"),
					TextAlignment = TextAlignment.Center,
					VerticalAlignment = VerticalAlignment.Bottom,
					Margin = new Thickness(0, 0, 0, 10)
				};

				var affixesStringList = new List<string>();

				foreach (var affix in boss.Affixes)
				{
					string affixString = string.Concat(affix.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

					affixesStringList.Add(affixString);
				}

				block3.Text = string.Join(" / ", affixesStringList);

				grid.Children.Add(block);
				grid.Children.Add(block2);
				grid.Children.Add(block3);

				button.Content = grid;

				button.Tag = boss;

				button.Click += BossButton_Click;

				DungeonSelectPanel.Children.Add(button);
			}
		}

		private static DungeonGroup GetDungeonGroup(Button dungeonGroupButton)
		{
			return dungeonGroupButton.Tag as DungeonGroup;
		}

		private static Dungeon GetDungeon(Button dungeonButton)
		{
			return dungeonButton.Tag as Dungeon;
		}

		private static Boss GetBoss(Button bossButton)
		{
			return bossButton.Tag as Boss;
		}

		private void ResetSelection()
		{
			_bossSelected = null;
			_dungeonGroupSelected = null;
			_dungeonSelected = null;
			UndoButton.Visibility = Visibility.Hidden;
		}

		private void DungeonGroupButton_Click(object sender, RoutedEventArgs e)
		{
			// Check if any quest is currently assigned to this hero (if so, hero can't enter the dungeon).
			if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
			{
				_dungeonGroupSelected = GetDungeonGroup(sender as Button);
				LoadDungeonSelectionInterface();

				(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon";
			}
		}

		private void DungeonButton_Click(object sender, RoutedEventArgs e)
		{
			_dungeonSelected = GetDungeon(sender as Button);
			LoadBossSelectionInterface();

			(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting boss";
		}

		private void BossButton_Click(object sender, RoutedEventArgs e)
		{
			_bossSelected = GetBoss(sender as Button);

			if (!CheckAndRemoveDungeonKeys())
			{
				AlertBox.Show("Not enough dungeon keys to enter.\nTry to get them by completing quests and killing monsters!", MessageBoxButton.OK);
				return;
			}

			SetupBossFight();

			ResetAndLoadDungeonGroupSelectionInterface();
		}

		private bool CheckAndRemoveDungeonKeys()
		{
			// Check if user has enough dungeon keys to enter boss fight.
			var counts = _dungeonGroupSelected.KeyRequirementRarities.GroupBy(x => x).ToDictionary(k => k.Key, v => v.Count());

			foreach (var pair in counts)
			{
				if (User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == pair.Key).Quantity < pair.Value)
				{
					return false;
				}
			}

			// Remove dungeon keys from Player.
			foreach (var pair in counts)
			{
				User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == pair.Key).Quantity -= pair.Value;
			}

			return true;
		}

		private void SetupBossFight()
		{
			InterfaceController.ChangePage(GameAssets.Pages["DungeonBoss"], "Boss fight");

			(GameAssets.Pages["DungeonBoss"] as DungeonBossPage)?.StartBossFight(_bossSelected.CopyEnemy());
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

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceController.ChangePage(GameAssets.Pages["Town"], "Town");
			ResetAndLoadDungeonGroupSelectionInterface();
		}
	}
}