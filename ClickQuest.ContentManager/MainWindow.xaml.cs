using ClickQuest.ContentManager.UserInterface;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.ContentManager
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
				var currentTabNameAsContentType = (ContentType)Enum.Parse(typeof(ContentType), currentTabName);

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
				}
			}
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			ContentSaver.SaveAllContent();
		}
	}
}