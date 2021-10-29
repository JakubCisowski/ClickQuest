using System.Globalization;
using System.Windows;
using ClickQuest.ContentManager.GameData;

namespace ClickQuest.ContentManager
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			// Set default culture for all threads for this application (affects date and string formats, e.g. periods instead of commas)
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

			JsonFilePaths.CalculateGameAssetsFilePaths();
			
			ContentLoader.LoadAllContent();
		}
	}
}