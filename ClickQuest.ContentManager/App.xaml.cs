using System.Globalization;
using System.Windows;
using ClickQuest.ContentManager.Logic.Helpers;
using ClickQuest.ContentManager.Logic.Models;

namespace ClickQuest.ContentManager;

public partial class App : Application
{
	protected void Application_Startup(object sender, StartupEventArgs e)
	{
		// Set default culture for all threads for this application (affects date and string formats, e.g. periods instead of commas)
		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

		FilePathHelper.CalculateGameAssetsFilePaths();
		
		/* Content seeding - use with caution!
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.ArtifactsFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.BlessingsFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.BossesFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.DungeonsFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.DungeonGroupsFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.MonstersFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.RegionsFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.QuestsFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.RecipesFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.PriestOfferFilePath);
		GameAssetsHelper.SeedContent<Artifact>(FilePathHelper.ShopOfferFilePath);
		*/

		GameAssetsHelper.LoadAllContent();
	}
}