using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Helpers;
using ClickQuest.ContentManager.UserInterface.Panels;

namespace ClickQuest.ContentManager.UserInterface.Windows;

public partial class MainWindow : Window
{
	private UserControl _currentPanel;

	public MainWindow()
	{
		InitializeComponent();
	}

	private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (e.Source is TabControl tabControl)
		{
			SaveObjectChanges();

			var currentTabName = (tabControl.SelectedItem as TabItem).Header.ToString();
			var currentTabNameAsContentType = (AssetType)Enum.Parse(typeof(AssetType), currentTabName.Replace(" ", ""));

			switch (currentTabNameAsContentType)
			{
				case AssetType.Artifacts:
					var artifactsPanel = new ArtifactsPanel();
					_currentPanel = artifactsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(artifactsPanel);
					break;

				case AssetType.Regions:
					var regionsPanel = new RegionsPanel();
					_currentPanel = regionsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(regionsPanel);
					break;

				case AssetType.Materials:
					var materialsPanel = new MaterialsPanel();
					_currentPanel = materialsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(materialsPanel);
					break;

				case AssetType.Recipes:
					var recipesPanel = new RecipesPanel();
					_currentPanel = recipesPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(recipesPanel);
					break;

				case AssetType.Blessings:
					var blessingsPanel = new BlessingsPanel();
					_currentPanel = blessingsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(blessingsPanel);
					break;

				case AssetType.Bosses:
					var bossesPanel = new BossesPanel();
					_currentPanel = bossesPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(bossesPanel);
					break;

				case AssetType.DngGroups:
					var dngGroupsPanel = new DungeonGroupsPanel();
					_currentPanel = dngGroupsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(dngGroupsPanel);
					break;

				case AssetType.DngKeys:
					var dngKeysPanel = new DungeonKeysPanel();
					_currentPanel = dngKeysPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(dngKeysPanel);
					break;

				case AssetType.Dungeons:
					var dungeonsPanel = new DungeonsPanel();
					_currentPanel = dungeonsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(dungeonsPanel);
					break;

				case AssetType.Ingots:
					var ingotsPanel = new IngotsPanel();
					_currentPanel = ingotsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(ingotsPanel);
					break;

				case AssetType.Monsters:
					var monstersPanel = new MonstersPanel();
					_currentPanel = monstersPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(monstersPanel);
					break;

				case AssetType.Quests:
					var questsPanel = new QuestsPanel();
					_currentPanel = questsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(questsPanel);
					break;

				case AssetType.Priest:
					var priestPanel = new PriestOfferPatternsPanel();
					_currentPanel = priestPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(priestPanel);
					break;

				case AssetType.Shop:
					var shopPanel = new ShopOfferPatternsPanel();
					_currentPanel = shopPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(shopPanel);
					break;

				case AssetType.GameMech:
					var gameMechanicsPanel = new GameMechanicsTabsPanel();
					_currentPanel = gameMechanicsPanel;

					(tabControl.SelectedContent as Grid)?.Children.Clear();
					(tabControl.SelectedContent as Grid)?.Children.Add(gameMechanicsPanel);
					break;
			}
		}
	}

	private void SaveObjectChanges()
	{
		if (_currentPanel is null)
		{
			return;
		}

		switch (_currentPanel)
		{
			case ArtifactsPanel artifactsPanel:
				artifactsPanel.Save();
				break;

			case RegionsPanel regionsPanel:
				regionsPanel.Save();
				break;

			case MaterialsPanel materialsPanel:
				materialsPanel.Save();
				break;

			case RecipesPanel recipesPanel:
				recipesPanel.Save();
				break;

			case BlessingsPanel blessingsPanel:
				blessingsPanel.Save();
				break;

			case BossesPanel bossesPanel:
				bossesPanel.Save();
				break;

			case DungeonGroupsPanel dungeonGroupsPanel:
				dungeonGroupsPanel.Save();
				break;

			case DungeonKeysPanel dungeonKeysPanel:
				dungeonKeysPanel.Save();
				break;

			case IngotsPanel ingotsPanel:
				ingotsPanel.Save();
				break;

			case MonstersPanel monstersPanel:
				monstersPanel.Save();
				break;

			case QuestsPanel questsPanel:
				questsPanel.Save();
				break;

			case PriestOfferPatternsPanel priestPanel:
				priestPanel.Save();
				break;

			case ShopOfferPatternsPanel shopPanel:
				shopPanel.Save();
				break;

			case GameMechanicsTabsPanel gameMechanicsPanel:
				gameMechanicsPanel.Save();
				break;
		}
	}

	private void MainWindow_OnClosing(object sender, CancelEventArgs e)
	{
		SaveObjectChanges();

		GameAssetsHelper.SaveAllContent();

		ValidationHelper.ValidateData();
	}
}