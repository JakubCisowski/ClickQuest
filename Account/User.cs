using ClickQuest.Heroes;
using ClickQuest.Items;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Account
{
    public partial class User : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        private static User _instance;

        public static User Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new User();
                }
                return _instance;
            }
        }

        private List<Hero> _heroes;
        private List<Item> _items;
        private Hero _currentHero;

        public List<Hero> Heroes
        {
            get
            {
                return _heroes;
            }
            set
            {
                _heroes = value;
                OnPropertyChanged();
            }
        }

        public List<Item> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public Hero CurrentHero
        {
            get
            {
                return _currentHero;
            }
            set
            {
                _currentHero = value;
                OnPropertyChanged();
            }
        }

        static User()
        {
            Data.Database.Load();
        }

        private User()
        {
            _heroes = new List<Hero>();
            _items = new List<Item>();
        }
    }
}