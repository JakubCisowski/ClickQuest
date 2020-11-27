using ClickQuest.Heroes;
using ClickQuest.Items;
using System.Collections.Generic;

namespace ClickQuest.Account
{
	public static partial class User
	{
		private static List<Hero> _heroes;
        private static List<Item> _items;

        private static Hero _currentHero;

		public static List<Hero> Heroes
		{
			get
			{
				return _heroes;
			}
			set
			{
				_heroes = value;
			}
		}

		public static List<Item> Items
		{
			get
			{
                return _items;
            }
			set
			{
                _items = value;
            }
		}

		public static Hero CurrentHero
		{
			get
			{
				return _currentHero;
			}
			set
			{
				_currentHero = value;
			}
		}

		static User()
		{
			_heroes = new List<Hero>();
            _items = new List<Item>();
            Data.Database.Load();
		}
	}
}