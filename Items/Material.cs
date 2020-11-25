using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClickQuest.Items
{
    public partial class Material : INotifyPropertyChanged
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
        private int _value;

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

        public int Value
        {
            get
            {
                return _value;
            }
        }

        #endregion
    }
}