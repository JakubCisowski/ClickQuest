using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Windows;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Game.UserInterface.Pages;

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

		var grid = new Grid();

		var col1 = new ColumnDefinition
		{
			Width = new GridLength(1, GridUnitType.Star)
		};
		var col2 = new ColumnDefinition
		{
			Width = new GridLength(1, GridUnitType.Star)
		};

		var row1 = new RowDefinition
		{
			Height = new GridLength(1, GridUnitType.Star)
		};
		var row2 = new RowDefinition
		{
			Height = new GridLength(1, GridUnitType.Star)
		};
		var row3 = new RowDefinition
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
			var button = new Button
			{
				Background = (SolidColorBrush)FindResource("BrushGray1"),
				Name = "DungeonGroup" + GameAssets.DungeonGroups[i].Id,
				Width = 250,
				Height = 100
			};

			var panel = new StackPanel
			{
				VerticalAlignment = VerticalAlignment.Center
			};

			var block = new TextBlock
			{
				FontSize = 22,
				Text = GameAssets.DungeonGroups[i].Name,
				TextAlignment = TextAlignment.Center
			};

			var border = new Border
			{
				BorderThickness = new Thickness(3),
				BorderBrush = ColorsHelper.GetRarityColor((Rarity)i),
				Width = 240,
				Height = 90
			};

			var panel2 = new StackPanel()
			{
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0,5,0,0)
			};

			var icon = new PackIcon
			{
				Width = 28,
				Height = 28,
				Foreground = ColorsHelper.GetRarityColor((Rarity)i),
				VerticalAlignment = VerticalAlignment.Center
			};
			icon.Kind = PackIconKind.Key;

			var block2 = new TextBlock
			{
				FontSize = 24,
				FontStyle = FontStyles.Italic,
				Text = "x1",
				TextAlignment = TextAlignment.Center,
				Margin = new Thickness(5,0,0,0)
			};


			panel.Children.Add(block);

			panel2.Children.Add(icon);
			panel2.Children.Add(block2);

			panel.Children.Add(panel2);

			border.Child = panel;

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
		foreach (var dungeon in dungeonsOfThisGroup)
		{
			var button = new Button
			{
				Background = (SolidColorBrush)FindResource("BrushGray1"),
				Name = "Dungeon" + dungeon.Id,
				Width = 330,
				Height = 100
			};

			var panel = new StackPanel();

			var block = new TextBlock
			{
				FontSize = 25,
				Text = dungeon.Name,
				TextAlignment = TextAlignment.Center,
				FontFamily = (FontFamily)FindResource("FontFancy")
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
		foreach (var bossId in _dungeonSelected.BossIds)
		{
			var boss = GameAssets.Bosses.FirstOrDefault(x => x.Id == bossId);

			var button = new Button
			{
				Background = (SolidColorBrush)FindResource("BrushGray1"),
				Name = "Boss" + boss.Id,
				Width = 330,
				Height = 120
			};

			var grid = new Grid
			{
				Width = 320,
				Height = 110
			};

			var block = new TextBlock
			{
				FontSize = 25,
				Text = boss.Name,
				TextAlignment = TextAlignment.Center,
				FontFamily = (FontFamily)FindResource("FontFancy"),
				VerticalAlignment = VerticalAlignment.Top,
				Margin = new Thickness(0, 10, 0, 0)
			};

			var block2 = new TextBlock
			{
				FontSize = 18,
				TextAlignment = TextAlignment.Center,
				FontFamily = (FontFamily)FindResource("FontFancy"),
				VerticalAlignment = VerticalAlignment.Center
			};

			block2.Inlines.Add(new Run("Health: "));
			block2.Inlines.Add(new Run(boss.Health.ToString())
			{
				Foreground = (SolidColorBrush)FindResource("BrushRed")
			});

			var block3 = new TextBlock
			{
				FontSize = 16,
				FontFamily = (FontFamily)FindResource("FontRegularItalic"),
				TextAlignment = TextAlignment.Center,
				VerticalAlignment = VerticalAlignment.Bottom,
				Margin = new Thickness(0, 0, 0, 10)
			};

			var affixesStringList = new List<string>();

			foreach (var affix in boss.Affixes)
			{
				var affixString = string.Concat(affix.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

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
		_dungeonGroupSelected = GetDungeonGroup(sender as Button);
		LoadDungeonSelectionInterface();

		(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting dungeon";
	}

	private void DungeonButton_Click(object sender, RoutedEventArgs e)
	{
		_dungeonSelected = GetDungeon(sender as Button);
		LoadBossSelectionInterface();

		(Window.GetWindow(this) as GameWindow).LocationInfo = "Selecting boss";
	}

	private void BossButton_Click(object sender, RoutedEventArgs e)
	{
		// Check if any quest is currently assigned to this hero (if so, hero can't enter the dungeon).
		if (User.Instance.CurrentHero.Quests.All(x => x.EndDate == default))
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
		else
		{
			AlertBox.Show("Your hero is busy completing quest!\nCheck back when it's finished.", MessageBoxButton.OK);
		}
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
		InterfaceHelper.ChangePage(GameAssets.Pages["DungeonBoss"], "Boss fight");

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
		InterfaceHelper.ChangePage(GameAssets.Pages["Town"], "Town");
		ResetAndLoadDungeonGroupSelectionInterface();
	}
}