using System;
using System.Windows;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Data;
using ClickQuest.Game.UserInterface.Pages;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Load JSONs.
			GameAssetsLoader.Load();

			var gameWindow = new GameWindow();
			Current.MainWindow = gameWindow;
			gameWindow.Show();

			UserDataLoader.Load();

			(GameAssets.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}
	}
}