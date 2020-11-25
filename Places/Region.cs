using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using ClickQuest.Enemies;

namespace ClickQuest.Places
{
    public class Region : INotifyPropertyChanged
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
        private string _background;
        private List<Monster> _monsters;

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

        #endregion
	}
}