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
			var dataProperties = typeof(GameData).GetProperties();

			foreach (var propertyInfo in dataProperties)
			{
				var propertyValue = propertyInfo.GetValue(null) as IEnumerable;
				var collectionType = propertyInfo.PropertyType.GetGenericArguments().FirstOrDefault();

				InvokeIdUniquenessChecking(collectionType, propertyValue);
			}
		}

		private static void InvokeIdUniquenessChecking(Type collectionType, IEnumerable propertyValue)
		{
			bool isCollectionIdentifiable = typeof(IIdentifiable).IsAssignableFrom(collectionType);
			if (isCollectionIdentifiable)
			{
				var convertedCollection = propertyValue.Cast<IIdentifiable>().ToList();
				var methodInfo = typeof(DataValidator).GetMethod("CheckIdUniqueness");
				methodInfo.Invoke(null, new object[] {convertedCollection, collectionType});
			}
		}

		public static void CheckIdUniqueness(List<IIdentifiable> collection, Type collectionType)
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
	}
}