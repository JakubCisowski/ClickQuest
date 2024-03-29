using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Helpers;
using ClickQuest.Game.UserInterface.Helpers.ToolTips;

namespace ClickQuest.Game.UserInterface.Pages;

public partial class InfoPage
{
	private readonly Page _previousPage;
	private readonly string _previousLocationInfo;

	public InfoPage(Page previousPage, string previousLocationInfo)
	{
		InitializeComponent();

		_previousPage = previousPage;
		_previousLocationInfo = previousLocationInfo;

		RegionsListView.ItemsSource = GameAssets.Regions.Select(x => x.Name);
		DungeonsListView.ItemsSource = GameAssets.Dungeons.Select(x => x.Name);
		GameMechanicsListView.ItemsSource = GameAssets.GameMechanicsTabs.Select(x => x.Name);
	}

	private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
	{
		InterfaceHelper.ChangePage(_previousPage, _previousLocationInfo);
	}

	private void RegionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count == 0)
		{
			return;
		}

		var region = GameAssets.Regions.FirstOrDefault(x => x.Name == e.AddedItems[0]?.ToString());

		if (region != null)
		{
			GenerateRegionInfoPanel(region);

			// Clear the selected Dungeons and GameMechanics when choosing a Region.
			DungeonsListView.SelectedItems.Clear();
			GameMechanicsListView.SelectedItems.Clear();
		}
	}

	private void DungeonsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count == 0)
		{
			return;
		}

		var dungeon = GameAssets.Dungeons.FirstOrDefault(x => x.Name == e.AddedItems[0]?.ToString());

		if (dungeon != null)
		{
			GenerateDungeonInfoPanel(dungeon);

			// Clear the selected Regions and GameMechanics when choosing a Dungeon.
			RegionsListView.SelectedItems.Clear();
			GameMechanicsListView.SelectedItems.Clear();
		}
	}

	private void GenerateRegionInfoPanel(Region region)
	{
		InfoPanel.Children.Clear();

		var regionNameBlock = new TextBlock
		{
			Text = region.Name,
			FontSize = 26,
			FontFamily = (FontFamily)FindResource("FontFancy"),
			TextAlignment = TextAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5)
		};

		var levelRequirementBlock = new TextBlock
		{
			Text = "Level Requirement: " + region.LevelRequirement,
			FontSize = 20,
			TextAlignment = TextAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5)
		};

		var descriptionBlock = new TextBlock
		{
			Text = region.Description,
			FontSize = 18,
			TextAlignment = TextAlignment.Justify,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5),
			TextWrapping = TextWrapping.Wrap,
			FontFamily = (FontFamily)FindResource("FontFancy")
		};

		var separator = new Separator
		{
			Height = 2,
			Width = 400,
			Margin = new Thickness(30)
		};

		InfoPanel.Children.Add(regionNameBlock);
		InfoPanel.Children.Add(levelRequirementBlock);
		InfoPanel.Children.Add(descriptionBlock);
		InfoPanel.Children.Add(separator);

		var sortedMonsterPatterns = region.MonsterSpawnPatterns.OrderByDescending(x => GameAssets.BestiaryEntries.Any(y => y.Id == x.MonsterId && y.EntryType == BestiaryEntryType.Monster)).ThenByDescending(z => z.Frequency);

		foreach (var monsterPattern in sortedMonsterPatterns)
		{
			var monster = monsterPattern.Monster;
			var monsterDiscovered = GameAssets.BestiaryEntries.Any(x => x.Id == monster.Id && x.EntryType == BestiaryEntryType.Monster);

			var monsterNameBlock = new TextBlock
			{
				FontSize = 22,
				FontFamily = (FontFamily)FindResource("FontFancy"),
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			if (!monsterDiscovered)
			{
				monsterNameBlock.Text = "Unknown Monster";

				monsterNameBlock.ToolTip = GeneralToolTipHelper.GenerateUndiscoveredEnemyToolTip();
			}
			else
			{
				monsterNameBlock.Text = monster.Name;
			}

			InfoPanel.Children.Add(monsterNameBlock);

			if (!monsterDiscovered)
			{
				var monsterSeparator = new Separator
				{
					Height = 2,
					Width = 200,
					Margin = new Thickness(10)
				};

				InfoPanel.Children.Add(monsterSeparator);

				continue;
			}

			var monsterHealthBlock = new TextBlock
			{
				Text = "Health: " + monster.Health,
				FontSize = 16,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			InfoPanel.Children.Add(monsterHealthBlock);

			var monsterDescriptionBlock = new TextBlock
			{
				Text = monster.Description,
				FontSize = 16,
				TextAlignment = TextAlignment.Justify,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5),
				TextWrapping = TextWrapping.Wrap
			};

			InfoPanel.Children.Add(monsterDescriptionBlock);

			var lootLabelBlock = new TextBlock
			{
				Text = "Loot: ",
				FontSize = 18,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			InfoPanel.Children.Add(lootLabelBlock);

			var sortedLootPatterns = monster.MonsterLootPatterns.OrderByDescending(x => GameAssets.BestiaryEntries.Any(y => y.Id == x.MonsterLootId && y.EntryType == BestiaryEntryType.MonsterLoot)).ThenBy(y => y.Item.Rarity);

			foreach (var lootPattern in sortedLootPatterns)
			{
				var item = lootPattern.Item;
				var monsterLootDiscovered = GameAssets.BestiaryEntries.Any(x => x.Id == item.Id && x.RelatedEnemyId == monster.Id && x.EntryType == BestiaryEntryType.MonsterLoot);

				var itemNameBlock = new TextBlock
				{
					TextAlignment = TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5),
					Foreground = ColorsHelper.GetRarityColor(item.Rarity),
					FontSize = 16
				};

				if (!monsterLootDiscovered)
				{
					itemNameBlock.Text = "Unknown " + item.RarityString + " " + lootPattern.MonsterLootType;
					itemNameBlock.FontFamily = (FontFamily)FindResource("FontRegularItalic");
					itemNameBlock.ToolTip = ItemToolTipHelper.GenerateUndiscoveredItemToolTip();
				}
				else
				{
					itemNameBlock.Text = item is Recipe recipe ? recipe.FullName : item.Name;
					itemNameBlock.FontFamily = (FontFamily)FindResource("FontRegularBold");
					itemNameBlock.ToolTip = ItemToolTipHelper.GenerateItemToolTip(item);
				}

				InfoPanel.Children.Add(itemNameBlock);
			}

			var itemSeparator = new Separator
			{
				Height = 2,
				Width = 200,
				Margin = new Thickness(15)
			};

			InfoPanel.Children.Add(itemSeparator);
		}
	}

	private void GenerateDungeonInfoPanel(Dungeon dungeon)
	{
		InfoPanel.Children.Clear();

		var dungeonNameBlock = new TextBlock
		{
			Text = dungeon.Name,
			FontSize = 26,
			FontFamily = (FontFamily)FindResource("FontFancy"),
			TextAlignment = TextAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5)
		};

		var dungeonGroupBlock = new TextBlock
		{
			Text = dungeon.DungeonGroup.Name,
			FontSize = 20,
			TextAlignment = TextAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5),
			Foreground = ColorsHelper.GetRarityColor((Rarity)dungeon.DungeonGroupId)
		};

		var descriptionBlock = new TextBlock
		{
			Text = dungeon.Description,
			FontSize = 18,
			TextAlignment = TextAlignment.Justify,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5),
			TextWrapping = TextWrapping.Wrap,
			FontFamily = (FontFamily)FindResource("FontFancy")
		};

		var separator = new Separator
		{
			Height = 2,
			Width = 400,
			Margin = new Thickness(30)
		};

		InfoPanel.Children.Add(dungeonNameBlock);
		InfoPanel.Children.Add(dungeonGroupBlock);
		InfoPanel.Children.Add(descriptionBlock);
		InfoPanel.Children.Add(separator);

		var sortedBosses = GameAssets.Bosses.Where(x => dungeon.BossIds.Contains(x.Id)).OrderByDescending(y => GameAssets.BestiaryEntries.Any(z => z.Id == y.Id && z.EntryType == BestiaryEntryType.Boss));

		foreach (var boss in sortedBosses)
		{
			var bossDiscovered = GameAssets.BestiaryEntries.Any(x => x.Id == boss.Id && x.EntryType == BestiaryEntryType.Boss);

			var bossNameBlock = new TextBlock
			{
				FontSize = 22,
				FontFamily = (FontFamily)FindResource("FontFancy"),
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			if (!bossDiscovered)
			{
				bossNameBlock.Text = "Unknown Boss";

				bossNameBlock.ToolTip = GeneralToolTipHelper.GenerateUndiscoveredEnemyToolTip();
			}
			else
			{
				bossNameBlock.Text = boss.Name;
			}

			InfoPanel.Children.Add(bossNameBlock);

			if (!bossDiscovered)
			{
				var bossSeparator = new Separator
				{
					Height = 2,
					Width = 200,
					Margin = new Thickness(10)
				};

				InfoPanel.Children.Add(bossSeparator);

				continue;
			}

			var bossAffixesStrings = new List<string>();

			foreach (var affix in boss.Affixes)
			{
				var affixString = string.Concat(affix.ToString().Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
				bossAffixesStrings.Add(affixString);
			}

			var bossAffixBlock = new TextBlock
			{
				Text = "Affixes: " + string.Join(" / ", bossAffixesStrings),
				FontSize = 16,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			InfoPanel.Children.Add(bossAffixBlock);

			var bossHealthBlock = new TextBlock
			{
				Text = "Health: " + boss.Health,
				FontSize = 16,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			InfoPanel.Children.Add(bossHealthBlock);

			var bossDescriptionBlock = new TextBlock
			{
				Text = boss.Description,
				FontSize = 16,
				TextAlignment = TextAlignment.Justify,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5),
				TextWrapping = TextWrapping.Wrap
			};

			InfoPanel.Children.Add(bossDescriptionBlock);

			var lootLabelBlock = new TextBlock
			{
				Text = "Loot: ",
				FontSize = 18,
				TextAlignment = TextAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(5)
			};

			InfoPanel.Children.Add(lootLabelBlock);

			var sortedLootPatterns = boss.BossLootPatterns.OrderByDescending(x => GameAssets.BestiaryEntries.Any(y => y.Id == x.BossLootId && y.EntryType == BestiaryEntryType.BossLoot)).ThenBy(z => z.Item?.Rarity);

			foreach (var lootPattern in sortedLootPatterns)
			{
				var item = lootPattern.Item;
				bool bossLootDiscovered;
				
				var itemNameBlock = new TextBlock
				{
					TextAlignment = TextAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center,
					Margin = new Thickness(5),
					FontSize = 16
				};

				// If item is null, then the LootPattern represents a Blessing.
				// Todo: rework this cringe
				if (item is null)
				{
					var blessing = GameAssets.Blessings.FirstOrDefault(x => x.Id == lootPattern.BossLootId);
					bossLootDiscovered = GameAssets.BestiaryEntries.Any(x => x.Id == blessing.Id && x.RelatedEnemyId == boss.Id && x.EntryType == BestiaryEntryType.BossLoot && x.LootType == RewardType.Blessing);

					itemNameBlock.Foreground = ColorsHelper.GetRarityColor(blessing.Rarity);

					if (!bossLootDiscovered)
					{
						itemNameBlock.Text = "Unknown " + blessing.RarityString + " " + lootPattern.BossLootType;
						itemNameBlock.FontFamily = (FontFamily)FindResource("FontRegularItalic");
						itemNameBlock.ToolTip = ItemToolTipHelper.GenerateUndiscoveredItemToolTip();
					}
					else
					{
						itemNameBlock.Text = blessing.Name;
						itemNameBlock.FontFamily = (FontFamily)FindResource("FontRegularBold");
						
						itemNameBlock.ToolTip = ItemToolTipHelper.GenerateBlessingToolTip(blessing);
					}
				}
				else
				{
					bossLootDiscovered = GameAssets.BestiaryEntries.Any(x => x.Id == item.Id && x.RelatedEnemyId == boss.Id && x.EntryType == BestiaryEntryType.BossLoot && x.LootType == lootPattern.BossLootType);
					
					itemNameBlock.Foreground = ColorsHelper.GetRarityColor(item.Rarity);
					
					if (!bossLootDiscovered)
					{
						itemNameBlock.Text = "Unknown " + item.RarityString + " " + lootPattern.BossLootType;
						itemNameBlock.FontFamily = (FontFamily)FindResource("FontRegularItalic");
						itemNameBlock.ToolTip = ItemToolTipHelper.GenerateUndiscoveredItemToolTip();
					}
					else
					{
						itemNameBlock.Text = item is Recipe recipe ? recipe.FullName : item.Name;
						itemNameBlock.FontFamily = (FontFamily)FindResource("FontRegularBold");
						itemNameBlock.ToolTip = ItemToolTipHelper.GenerateItemToolTip(item);
					}
				}

				InfoPanel.Children.Add(itemNameBlock);
			}

			var itemSeparator = new Separator
			{
				Height = 2,
				Width = 200,
				Margin = new Thickness(15)
			};

			InfoPanel.Children.Add(itemSeparator);
		}
	}

	private void GameMechanicsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count == 0)
		{
			return;
		}
		
		// Clear the selected Regions and Dungeons when choosing a Game Mechanic.
		RegionsListView.SelectedItems.Clear();
		DungeonsListView.SelectedItems.Clear();
		
		InfoPanel.Children.Clear();

		var selectedName = e.AddedItems[0]?.ToString();

		var description = GameAssets.GameMechanicsTabs.FirstOrDefault(x => x.Name == selectedName)?.Description;

		var nameTextBlock = new TextBlock
		{
			Text = selectedName,
			FontSize = 24,
			FontFamily = (FontFamily)FindResource("FontFancy"),
			TextAlignment = TextAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5)
		};

		var separator = new Separator
		{
			Height = 2,
			Width = 400,
			Margin = new Thickness(10)
		};

		var descriptionTextBlock = new TextBlock
		{
			FontSize = 18,
			TextAlignment = TextAlignment.Justify,
			HorizontalAlignment = HorizontalAlignment.Center,
			Margin = new Thickness(5),
			TextWrapping = TextWrapping.Wrap,
		};

		descriptionTextBlock.Inlines.AddRange(DescriptionsHelper.GenerateDescriptionInlines(description, (SolidColorBrush)Application.Current.FindResource("BrushBlack")));

		InfoPanel.Children.Add(nameTextBlock);
		InfoPanel.Children.Add(separator);
		InfoPanel.Children.Add(descriptionTextBlock);
	}
}