using System.Windows;

namespace ClickQuest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void Application_Startup(object sender, StartupEventArgs e)
        {
			//using (var db = new Entity.UserContext())
			//{
           //     db.Database.EnsureCreated();
            //}

            //Entity.EntityOperations.LoadGame();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            //Entity.EntityOperations.SaveGame();
        }
    }
}