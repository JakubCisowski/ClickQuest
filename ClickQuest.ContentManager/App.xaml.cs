using System.Windows;

namespace ClickQuest.ContentManager
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			ContentLoader.CalculateGameAssetsFilePaths();
			
			ContentLoader.LoadAllContent();
		}
	}
}