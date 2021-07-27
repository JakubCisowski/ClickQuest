using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClickQuest.Data;
using ClickQuest.Interfaces;
using ClickQuest.Items;

namespace ClickQuest.Extensions.ValidationManager
{
	public static class DataValidator
	{
		public static void ValidateData()
		{
			CheckIdUniqueness();
			CheckAllFrequenciesCorrectness();
			// Does id of referenced collection exist?
		}

		private static void CheckIdUniqueness()
		{
			var dataProperties = typeof(GameData).GetProperties();

			foreach (var propertyInfo in dataProperties)
			{
				var propertyValue = propertyInfo.GetValue(null) as IEnumerable;
				var collectionType = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();

				bool isCollectionIdentifiable = typeof(IIdentifiable).IsAssignableFrom(collectionType);
				if (isCollectionIdentifiable)
				{
					var convertedCollection = propertyValue.Cast<IIdentifiable>().ToList();
					var methodInfo = typeof(DataValidator).GetMethod("CheckCollectionIdUniqueness");
					methodInfo.Invoke(null, new object[] { convertedCollection, collectionType });
				}
			}
		}

		public static void CheckCollectionIdUniqueness(List<IIdentifiable> collection, Type collectionType)
		{
			var idList = collection.Select(x => x.Id);

			var duplicates = idList.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

			bool areIdsUnique = duplicates.Count == 0;
			if (!areIdsUnique)
			{
				string message = $"Following Id's of type '{collectionType}' are not unique: ";

				var duplicatedValues = duplicates.Distinct();
				foreach(var value in duplicatedValues)
				{
					message += value + ",";
				}
				
				Logger.Log(message);
			}
		}

		private static void CheckAllFrequenciesCorrectness()
		{
			GameData.Monsters.ForEach(x=>CheckFrequencySum(x.Name, x.Loot.Select(y=>y.Frequency)));
			GameData.Regions.ForEach(x=>CheckFrequencySum(x.Name, x.Monsters.Select(y=>y.Frequency)));
			GameData.Bosses.ForEach(x=>x.BossLoot.ForEach(y=>CheckIfFrequenciesAreValid(x.Name, y.Frequencies)));
		}

		private static void CheckFrequencySum(string objectName, IEnumerable<double> frequencies)
		{
			double frequenciesSum = frequencies.Sum();
			if(frequenciesSum != 1)
			{
				string message = $"'{objectName}' frequencies do not add up to 1";
				Logger.Log(message);
			}
		}

		private static void CheckIfFrequenciesAreValid(string objectName, IEnumerable<double> frequencies)
		{
			bool areFrequenciesInvalid = frequencies.Any(x => x < 0 || x > 1);

			if(areFrequenciesInvalid)
			{
				string message = $"'{objectName}' frequency value is greater than 1";
				Logger.Log(message);
			}
		}
	}
}