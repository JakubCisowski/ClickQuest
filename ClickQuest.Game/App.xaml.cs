using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Data;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Pages;
using ClickQuest.Game.UserInterface.Windows;

namespace ClickQuest.Game
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Set crash logs output to Logs - any unhandled exceptions will be saved to a text file locally.
			Application.Current.DispatcherUnhandledException += OnUnhandledException;

			// Set default culture for all threads for this application (affects date and string formats, e.g. periods instead of commas)
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

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

		private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
		{
			var e = args.Exception;

			string filePath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString(), "Logs", $"CrashLog {DateTime.Now.ToString("ddMMyyyy HHmmss")}.txt");

			using (var writer = new StreamWriter(filePath))
			{
				writer.WriteLine($"Exception message: {e.Message}");
				writer.WriteLine($"Inner exception: {e.InnerException}");
				writer.WriteLine($"Stack trace: {e.StackTrace}");
				writer.WriteLine();
				writer.WriteLine("User.json file:");
				writer.WriteLine(UserDataLoader.Save());
			}
		}
	}
}