using System;
using System.Collections.Generic;
using System.Linq;
using ClickQuest.Game.Models;
using static ClickQuest.Game.Helpers.RandomnessHelper;

namespace ClickQuest.Game.Helpers;

public static class CollectionsHelper
{
	public static void InitializeDictionary<T1, T2>(IDictionary<T1, T2> collection) where T1 : Enum
	{
		var enumValues = Enum.GetValues(typeof(T1));

		for (var i = 0; i < enumValues.Length; i++)
		{
			collection.Add((T1)enumValues.GetValue(i), default);
		}
	}

	public static void AddItemToCollection<T>(Item itemToAdd, List<T> itemCollection, int amount = 1) where T : Item
	{
		foreach (var item in itemCollection)
		{
			if (item.Id == itemToAdd.Id)
			{
				item.Quantity += amount;
				return;
			}
		}

		// If user doesn't have this item, clone and add it.
		var copy = itemToAdd.CopyItem(amount);

		itemCollection.Add((T)copy);
	}

	public static void RemoveItemFromCollection<T>(Item itemToRemove, List<T> itemCollection, int amount = 1) where T : Item
	{
		foreach (var item in itemCollection)
		{
			if (item.Id == itemToRemove.Id)
			{
				item.Quantity -= amount;
				if (item.Quantity <= 0)
				{
					// Remove item from database.
					itemCollection.Remove(itemToRemove as T);
				}

				return;
			}
		}
		// If user doesn't have this item, don't do anything (check Item.Quantity).
	}

	public static int RandomizeFrequenciesListPosition(List<double> frequencies)
	{
		var randomizedValue = Rng.Next(1, 10001) / 10000d;
		var i = 0;

		while (randomizedValue > frequencies[i])
		{
			randomizedValue -= frequencies[i];
			i++;
		}

		return i;
	}

	public static List<T> ReorderItemsInList<T>(this List<T> specificEquipmentCollection) where T : Item
	{
		// 1. Items should be ordered based on (name / rarity / type / something else) - currently Name.
		var orderedItemsList = specificEquipmentCollection.OrderBy(x => x.Name).ToList();

		// 2. ???

		return orderedItemsList;
	}
}