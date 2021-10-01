using System;
using System.Linq;
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

		private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (e.Source is TabControl tabControl)
			{
				var currentTabName = (tabControl.SelectedItem as TabItem).Header;

				switch (currentTabName)
				{
					case "Artifacts":
						

						break;
				}
			}
		}

		private void ArtifactContentBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selectedName = (sender as ComboBox).SelectedValue.ToString();
			// var selectedArtifact = GameAssets.Artifacts.FirstOrDefault(x => x.Name == selectedName);
		}
	}
}