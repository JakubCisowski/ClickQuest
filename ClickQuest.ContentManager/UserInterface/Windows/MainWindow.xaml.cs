using ClickQuest.ContentManager.GameData;
using ClickQuest.ContentManager.UserInterface.Panels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.ContentManager.UserInterface.Windows
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl tabControl)
			{
				var currentTabName = (tabControl.SelectedItem as TabItem).Header.ToString();
				var currentTabNameAsContentType = (ContentType)Enum.Parse(typeof(ContentType), currentTabName.Replace(" ", ""));

				switch (currentTabNameAsContentType)
				{
					case ContentType.Artifacts:
						var artifactsPanel = new ArtifactsPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(artifactsPanel);
						break;

					case ContentType.Regions:
						var regionsPanel = new RegionsPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(regionsPanel);
						break;

					case ContentType.Materials:
						var materialsPanel = new MaterialsPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(materialsPanel);
						break;

					case ContentType.Recipes:
						var recipesPanel = new RecipesPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(recipesPanel);
						break;

					case ContentType.Blessings:
						var blessingsPanel = new BlessingsPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(blessingsPanel);
						break;

					case ContentType.Bosses:
						var bossesPanel = new BossesPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(bossesPanel);
						break;

					case ContentType.DngGroups:
						var dngGroupsPanel = new DungeonGroupsPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dngGroupsPanel);
						break;

					case ContentType.DngKeys:
						var dngKeysPanel = new DungeonKeysPanel();
						(tabControl.SelectedContent as Grid)?.Children.Clear();
						(tabControl.SelectedContent as Grid)?.Children.Add(dngKeysPanel);
						break;
				}
			}
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			ContentSaver.SaveAllContent();
		}
	}
}