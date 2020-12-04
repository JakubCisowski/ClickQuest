using ClickQuest.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ClickQuest.Entity
{
	public static class EntityOperations
	{
		public static void SaveGame()
		{
			using (var db = new UserContext())
			{
				db.Users.RemoveRange(db.Users);

				db.Users.AddRange(Account.User.Instance);
				db.Entry(Account.User.Instance).State=EntityState.Modified;

				db.SaveChanges();
			}
		}

		public static void LoadGame()
		{
			using (var db = new UserContext())
			{
				// Load user.
				Account.User.Instance = db.Users.FirstOrDefault();
			}
		}
	}
}