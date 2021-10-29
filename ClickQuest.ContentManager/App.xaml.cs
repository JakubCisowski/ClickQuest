using System.Windows;
using ClickQuest.ContentManager.GameData;

namespace ClickQuest.ContentManager
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			JsonFilePaths.CalculateGameAssetsFilePaths();
			
			ContentLoader.LoadAllContent();
		}
	}
}