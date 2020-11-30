using ClickQuest.Items;
using System.Collections.Generic;
using System.Linq;

namespace ClickQuest.Entity
{
    public static class EntityOperations
    {
        public static void SaveGame()
        {
            using (var db = new UserContext())
            {
                db.Heroes.RemoveRange(db.Heroes);
                db.Materials.RemoveRange(db.Materials);
                db.Recipes.RemoveRange(db.Recipes);
                db.Artifacts.RemoveRange(db.Artifacts);

                db.Heroes.AddRange(Account.User.Instance.Heroes);
				
    			db.Materials.AddRange(Account.User.Instance.Items.Where(x => x is Material).Cast<Material>());
                db.Recipes.AddRange(Account.User.Instance.Items.Where(x => x is Recipe).Cast<Recipe>());
                db.Artifacts.AddRange(Account.User.Instance.Items.Where(x => x is Artifact).Cast<Artifact>());

                db.SaveChanges();
            }
        }

        public static void LoadGame()
        {
            using (var db = new UserContext())
            {
                Account.User.Instance.Heroes = db.Heroes.ToList();

                var list = new List<Item>();

                list.AddRange(db.Materials.ToList<Item>());
                list.AddRange(db.Recipes.ToList<Item>());
                list.AddRange(db.Artifacts.ToList<Item>());

                Account.User.Instance.Items = list;
            }
        }

        public static void ClearHeroes()
        {
            using (var db = new UserContext())
            {
                db.Heroes.RemoveRange(db.Heroes);
                db.Materials.RemoveRange(db.Materials);
                db.Recipes.RemoveRange(db.Recipes);
                db.Artifacts.RemoveRange(db.Artifacts);
                db.SaveChanges();
            }
        }
    }
}