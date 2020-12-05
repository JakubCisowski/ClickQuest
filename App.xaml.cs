using ClickQuest.Entity;
using ClickQuest.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ClickQuest
{
    public partial class App : Application
    {
        protected void Application_Startup(object sender, StartupEventArgs e)
        {
            using (var db = new UserContext())
            {
                db.Database.EnsureCreated();

                var user = db.Users.Include(x=>x.Ingots).FirstOrDefault();

				// If there are no ingots in the database, add them (seed).
                if (user.Ingots.Count() == 0)
				{
					var rarities = Enum.GetValues(typeof(Rarity));
					var ingots = new List<Ingot>();
					for (int i = 0; i < rarities.GetLength(0); i++)
					{
						user.Ingots.Add(new Ingot((Rarity)rarities.GetValue(i), 0));
					}

					db.Users.Update(user);
					db.SaveChanges();
				}
            }

            Data.Database.Load();
            Entity.EntityOperations.LoadGame();
        }
    }
}