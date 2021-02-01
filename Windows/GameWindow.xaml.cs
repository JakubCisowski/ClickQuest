using ClickQuest.Items;
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

		#region Events

		protected override void OnClosing(CancelEventArgs e)
		{
			// Pause all blessings.
			Blessing.PauseBlessings();

			Entity.EntityOperations.SaveGame();

			base.OnClosing(e);
		}

		#endregion
	}
}
