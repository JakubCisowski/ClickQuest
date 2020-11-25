using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClickQuest.Items;

namespace ClickQuest.Enemies
{
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
        private List<(Material,double)> _loot;

        #endregion

        #region Properties

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

        #endregion
    }
}