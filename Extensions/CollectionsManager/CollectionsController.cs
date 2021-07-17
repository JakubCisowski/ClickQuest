using System.Collections.Generic;
using ClickQuest.Items;

namespace ClickQuest.Extensions.CollectionsManager
{
	public static class CollectionsController
	{
		public static void AddItemToCollection<T>(Item itemToAdd, List<T> itemCollection) where T : Item
		{
			foreach (var item in itemCollection)
			{
				if (item.Id == itemToAdd.Id)
				{
					item.Quantity++;
					return;
				}
			}

			// If user doesn't have this item, clone and add it.
			var copy = itemToAdd.CopyItem(1);

			itemCollection.Add((T)copy);
		}

		public static void RemoveItemFromCollection<T>(Item itemToRemove, List<T> itemCollection) where T : Item
        {
            foreach (Item item in itemCollection)
            {
                if (item.Id == itemToRemove.Id)
                {
                    item.Quantity--;
                    if (item.Quantity <= 0)
                    {
                        // Remove item from database.
                        Entity.EntityOperations.RemoveItem(item);
                    }
                    return;
                }
            }
            // If user doesn't have this item, don't do anything (check Item.Quantity).
        }
	}
}