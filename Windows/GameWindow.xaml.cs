using ClickQuest.Account;
using ClickQuest.Pages;
using System.ComponentModel;
using System.Windows;

namespace ClickQuest
{
	public partial class GameWindow : Window
	{
		public GameWindow()
		{
			InitializeComponent();

			(Data.Database.Pages["MainMenu"] as MainMenuPage).GenerateHeroButtons();
			(Application.Current.MainWindow as GameWindow).CurrentFrame.Navigate(Data.Database.Pages["MainMenu"]);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			// End current blessings.
			for (int i = 0; i < User.Instance.Blessings.Count; i++)
			{
				User.Instance.Blessings[0].ChangeBuffStatus(false);
			}

			Entity.EntityOperations.SaveGame();

			base.OnClosing(e);
		}
	}
}
