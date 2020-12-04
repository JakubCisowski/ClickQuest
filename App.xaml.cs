using System;
using System.Linq;
using System.Windows;
using ClickQuest.Entity;
using ClickQuest.Items;

namespace ClickQuest
{
	public partial class App : Application
	{
		protected void Application_Startup(object sender, StartupEventArgs e)
		{
			using (var db = new UserContext())
			{
				db.Database.EnsureCreated();

				// If the database is empty (eg. it was just created), then fill it with data.
				if (db.Ingots.Count() == 0)
				{
					var rarities = Enum.GetValues(typeof(Rarity));

					for (int i = 0; i < rarities.GetLength(0); i++)
					{
						db.Ingots.Add(new Ingot((Rarity)rarities.GetValue(i), 0));
					}

					db.SaveChanges();
				}
			}

			Data.Database.Load();
			Entity.EntityOperations.LoadGame();
		}
	}
}