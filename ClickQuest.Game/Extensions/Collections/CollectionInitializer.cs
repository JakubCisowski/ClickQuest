using System;
using System.Collections.Generic;

namespace ClickQuest.Game.Extensions.Collections
{
	public static class CollectionInitializer
	{
		public static void InitializeDictionary<T1, T2>(IDictionary<T1, T2> collection) where T1 : Enum
		{
			var enumValues = Enum.GetValues(typeof(T1));

			for (int i = 0; i < enumValues.Length; i++)
			{
				collection.Add((T1) enumValues.GetValue(i), default);
			}
		}
	}
}