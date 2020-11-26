using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClickQuest.Items;

namespace ClickQuest.Enemies
{
    public enum MonsterType
	{
		Beast, Flying
	}
    public partial class Monster : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

        #region Private Fields
        private int _id;
		private string _name;
        private int _health;
        private string _image;
        private List<MonsterType> _types;
        private List<(Material,double)> _loot;

        #endregion

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
        
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
                OnPropertyChanged();
            }
        }

        public string Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }

        public List<MonsterType> Types
        {
            get
            {
                return _types;
            }
            set
            {
                _types = value;
                OnPropertyChanged();
            }
        }

        public List<(Material,double)> Loot
        {
            get
            {
                return _loot;
            }
            set
            {
                _loot = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public Monster(int id, string name, int health, string image, List<MonsterType> types, List<(Material,double)> loot)
        {
            Id = id;
            Name = name;
            Health = health;
            Types = types;
            Loot = loot;
        }
    }
}