using ClickQuest.Account;
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
                // Ensure the database exists - if not, create it.
                db.Database.EnsureCreated();

                var user = db.Users.Include(x => x.Ingots).Include(x => x.DungeonKeys).FirstOrDefault();

                // The seeding has to be done here, because otherwise we would need to set up relationships manually (using foreign keys, etc.)

                // If there are no dungeon keys in the database, add them (seed).
                if (user.DungeonKeys.Count() == 0)
                {
                    var rarities = Enum.GetValues(typeof(Rarity));
                    for (int i = 0; i < rarities.GetLength(0); i++)
                    {
                        user.DungeonKeys.Add(new DungeonKey((Rarity)rarities.GetValue(i), 0));
                    }
                }

                // If there are no ingots in the database, add them (seed).
                if (user.Ingots.Count() == 0)
                {
                    var rarities = Enum.GetValues(typeof(Rarity));
                    for (int i = 0; i < rarities.GetLength(0); i++)
                    {
                        user.Ingots.Add(new Ingot((Rarity)rarities.GetValue(i), 0));
                    }

                    //db.Users.Update(user);
                    //db.SaveChanges();
                }

                db.Users.Update(user);
                db.SaveChanges();
            }

            // Load JSONs and Entity.
            Data.Database.Load();
            Entity.EntityOperations.LoadGame();

            // Resume blessings (if there are any left).
            foreach (var blessing in User.Instance.Blessings)
            {
                blessing.ChangeBuffStatus(true);
            }
        }
    }
}