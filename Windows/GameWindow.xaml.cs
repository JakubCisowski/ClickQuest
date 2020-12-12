using ClickQuest.Pages;
using ClickQuest.Account;
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
			foreach (var blessing in User.Instance.Blessings)
			{
				blessing.ChangeBuffStatus(false);
			}

			Entity.EntityOperations.SaveGame();

			base.OnClosing(e);
		}
	}
}
