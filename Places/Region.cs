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
		private string _name = "testName";
        private string _background;
        private List<(Monster,double)> _monsters;

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

        public List<(Monster, double)> Monsters
        {
            get
            {
                return _monsters;
            }
            set
            {
                _monsters=value;
                OnPropertyChanged();
            }
        }

        #endregion
        

        public Region(int id)
        {
            // load region from database (using id)
        }
    }
	
}