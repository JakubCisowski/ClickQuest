using System;
using System.Globalization;
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
			// Set crash logs output to Logs - any unhandled exceptions will be saved to a text file locally.
			// Release only, because we want to see Exceptions in IDE during development.
#if RELEASE
			Application.Current.DispatcherUnhandledException += OnUnhandledException;
#endif

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

			(GameAssets.Pages["MainMenu"] as MainMenuPage).UpdateCreateHeroButton();
			(GameAssets.Pages["MainMenu"] as MainMenuPage).UpdateSelectOrDeleteHeroButtons();

			// Save current time as the application's start time (for achievement tracking).
			User.SessionStartDate = DateTime.Now;
		}

#if RELEASE
		private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
		{
			var e = args.Exception;
		
			string directoryPath = Path.Combine(Environment.CurrentDirectory, "Logs");
			string filePath = Path.Combine(directoryPath, $"CrashLog {DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss")}.txt");
		
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			using (var writer = new StreamWriter(filePath))
			{
				writer.WriteLine($"Exception message: {e.Message}");
				writer.WriteLine($"Inner exception: {e.InnerException}");
				writer.WriteLine($"Stack trace: {e.StackTrace}");
				writer.WriteLine();
				writer.WriteLine("User.json file:");
		
				//  If exception was thrown before loading User.json (Ingots.Count can't be null after loading)
				if (User.Instance.Ingots?.Count == 0)
				{
					var encryptedUserJson = File.ReadAllBytes(UserDataLoader.UserDataPath);
					var userJson = DataEncryptionController.DecryptJsonUsingAes(encryptedUserJson);
					writer.Write(userJson);
				}
				else
				{
					writer.Write(UserDataLoader.Save());
				}
			}
			
			AlertBox.Show("There was an error with the game. A log file has been generated in the Logs folder. To help us fix the issue, you can contribute the log file at X. We apologize for the inconvenience.\nYour data has been saved.", MessageBoxButton.OK);
		}
#endif
	}
}