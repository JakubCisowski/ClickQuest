using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Heroes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Pages
{
	public partial class MainMenuPage : Page
	{
		public MainMenuPage()
		{
			InitializeComponent();
			GenerateHeroButtons();
		}

		public void GenerateHeroButtons()
		{
			HeroesPanel.Children.Clear();

			for (int i = 0; i < Account.User.Instance.Heroes.Count; i++)
			{
				var selectHeroButton = new Button()
				{
					Name = "Hero" + Account.User.Instance.Heroes[i].Id,
					Content = Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
					Width = 250,
					Height = 50
				};

				var deleteHeroButton = new Button()
				{
					Name = "DeleteHero" + Account.User.Instance.Heroes[i].Id,
					Content = "Delete " + Account.User.Instance.Heroes[i].Name + ", " + Account.User.Instance.Heroes[i].ThisHeroClass.ToString() + " [" + Account.User.Instance.Heroes[i].Level + " lvl]",
					Width = 100,
					Height = 50
				};


				selectHeroButton.Click += SelectHeroButton_Click;

				deleteHeroButton.Click += DeleteHeroButton_Click;

				HeroesPanel.Children.Add(selectHeroButton);
				HeroesPanel.Children.Add(deleteHeroButton);
			}
		}

		#region Events

		private void CreateHeroButton_Click(object sender, RoutedEventArgs e)
		{
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Database.Pages["HeroCreation"]);
		}

		private void SelectHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var id = int.Parse((sender as Button).Name.Substring(4));
			var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
			Account.User.Instance.CurrentHero = hero;

			Data.Database.RefreshPages();
			(Window.GetWindow(this) as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["Town"]);
		}

		private void DeleteHeroButton_Click(object sender, RoutedEventArgs e)
		{
			var id = int.Parse((sender as Button).Name.Substring(10));
			var hero = Account.User.Instance.Heroes.Where(x => x.Id == id).FirstOrDefault();
			Account.User.Instance.Heroes.Remove(hero);

			GenerateHeroButtons();
		}

		#endregion
	}
}