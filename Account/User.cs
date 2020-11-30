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
        private int _gold;

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

        public int Gold
        {
            get
            {
                return _gold;
            }
            set
            {
                _gold = value;
                foreach (var Hero in Heroes)
                {
                    Hero.Gold = _gold;
                }
                //CurrentHero.Gold = _gold;

                OnPropertyChanged();
            }
        }

        private User()
        {
            _heroes = new List<Hero>();
            _items = new List<Item>();
        }

        public void AddItem(Item itemToAdd)
        {
            // If user does have this item, increase quantity
            foreach (var item in Items)
            {
                if (item.Id==itemToAdd.Id && item.GetType() == itemToAdd.GetType())
                {
                    item.Quantity++;
                    return;
                }
            }

           // If user doesn't have this item, add it
           Items.Add(itemToAdd);
        }

        public void RemoveItem(Item itemToAdd)
        {
            // If user does have this item, decrease quantity
            foreach (var item in Items)
            {
                if (item.Id==itemToAdd.Id && item.GetType() == itemToAdd.GetType())
                {
                    item.Quantity--;
                    // Item property will automatically delete it if quantity will set to 0 or lower 
                    return;
                }
            }

           // If user doesn't have this item, don't do anything
        }
    }
}