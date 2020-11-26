using ClickQuest.Controls;
using ClickQuest.Places;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class RegionPage : Page
	{
		private Region _region;
		private Random _rng = new Random();

		public RegionPage(Region currentRegion)
		{
			InitializeComponent();

			_region = currentRegion;
			this.DataContext = _region;

			CreateMonsterButton();
		}

		private void TownButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(new TownPage());
		}

		public void CreateMonsterButton()
		{
			var button = new MonsterButton(_region);
			this.RegionPanel.Children.Insert(1, button);
		}
	}
}