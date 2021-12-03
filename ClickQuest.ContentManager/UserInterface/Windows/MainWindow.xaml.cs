using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.UserInterface.Panels;
using ClickQuest.ContentManager.Validation;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
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
				ContentType currentTabNameAsContentType = (ContentType) Enum.Parse(typeof(ContentType), currentTabName.Replace(" ", ""));

				switch (currentTabNameAsContentType)
				{
					case ContentType.Artifacts:
						ArtifactsPanel artifactsPanel = new ArtifactsPanel();
						_currentPanel = artifactsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(artifactsPanel);
						break;

					case ContentType.Regions:
						RegionsPanel regionsPanel = new RegionsPanel();
						_currentPanel = regionsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(regionsPanel);
						break;

					case ContentType.Materials:
						MaterialsPanel materialsPanel = new MaterialsPanel();
						_currentPanel = materialsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(materialsPanel);
						break;

					case ContentType.Recipes:
						RecipesPanel recipesPanel = new RecipesPanel();
						_currentPanel = recipesPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(recipesPanel);
						break;

					case ContentType.Blessings:
						BlessingsPanel blessingsPanel = new BlessingsPanel();
						_currentPanel = blessingsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(blessingsPanel);
						break;

					case ContentType.Bosses:
						BossesPanel bossesPanel = new BossesPanel();
						_currentPanel = bossesPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(bossesPanel);
						break;

					case ContentType.DngGroups:
						DungeonGroupsPanel dngGroupsPanel = new DungeonGroupsPanel();
						_currentPanel = dngGroupsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dngGroupsPanel);
						break;

					case ContentType.DngKeys:
						DungeonKeysPanel dngKeysPanel = new DungeonKeysPanel();
						_currentPanel = dngKeysPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dngKeysPanel);
						break;

					case ContentType.Dungeons:
						DungeonsPanel dungeonsPanel = new DungeonsPanel();
						_currentPanel = dungeonsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dungeonsPanel);
						break;

					case ContentType.Ingots:
						IngotsPanel ingotsPanel = new IngotsPanel();
						_currentPanel = ingotsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(ingotsPanel);
						break;

					case ContentType.Monsters:
						MonstersPanel monstersPanel = new MonstersPanel();
						_currentPanel = monstersPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(monstersPanel);
						break;

					case ContentType.Quests:
						QuestsPanel questsPanel = new QuestsPanel();
						_currentPanel = questsPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(questsPanel);
						break;

					case ContentType.Priest:
						PriestOfferPatternsPanel priestPanel = new PriestOfferPatternsPanel();
						_currentPanel = priestPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(priestPanel);
						break;

					case ContentType.Shop:
						ShopOfferPatternsPanel shopPanel = new ShopOfferPatternsPanel();
						_currentPanel = shopPanel;

						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(shopPanel);
						break;

					case ContentType.GameMech:
						GameMechanicsTabsPanel gameMechanicsPanel = new GameMechanicsTabsPanel();
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

			ContentSaver.SaveAllContent();

			DataValidator.ValidateData();
		}
	}
}