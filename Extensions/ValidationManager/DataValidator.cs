using System;
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
			CheckIdUniqueness<Material>(GameData.Materials);
		}

		public static void CheckIdUniqueness<T>(List<T> collection) where T:IIdentifiable
		{
			var idList = collection.Select(x => x.Id);

			var duplicates = idList.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

			bool areIdsUnique = duplicates.Count == 0;
			if (!areIdsUnique)
			{
				string message = $"Following Id's of type '{typeof(T)}' are not unique: ";

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