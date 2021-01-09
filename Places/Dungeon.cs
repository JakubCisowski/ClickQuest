using ClickQuest.Enemies;
using ClickQuest.Items;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Places
{
    public class Dungeon : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        #region Private Fields

        private int _id;
        private int _rarityGroup;
        private string _name;
        private string _background;
        private string _description;
        private List<Monster> _bosses;

        #endregion Private Fields

        #region Properties

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public int RarityGroup
        {
            get
            {
                return _rarityGroup;
            }
            set
            {
                _rarityGroup=value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }

        public List<Monster> Bosses
        {
            get
            {
                return _bosses;
            }
            set
            {
                _bosses = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        public Dungeon(int id, int rarityGroup, string name, string background, string description, List<Monster> bosses)
        {
            Id = id;
            RarityGroup=rarityGroup;
            Name = name;
            Background = background;
            Description=description;
            Bosses=bosses;
        }
    }
}