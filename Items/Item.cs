using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ClickQuest.Items
{
    public partial class Item :INotifyPropertyChanged
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
        private Rarity _rarity;
        private int _quantity;

        #endregion

        #region Properties
		
		[Key]
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

		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				OnPropertyChanged();
			}
		}

		public Rarity Rarity
		{
			get
			{
				return _rarity;
			}
			set
			{
                _rarity = value;
				OnPropertyChanged();
            }
		}

		public int Quantity
		{
			get
			{
				return _quantity;
			}
			set
			{
                _quantity = value;
                OnPropertyChanged();
            }
		}

		public string RarityString
		{
			get
			{
				string str = Rarity.ToString() + ' ';

				for (int i=0;i<(int)Rarity;i++)
				{
                    str += "✩";
                }

				return str;
			}
		}

		#endregion

        public Item(int id, string name, Rarity rarity, int value)
		{
			Id = id;
			Name = name;
            Rarity = rarity;
            Value = value;
			Quantity = 0;
		}

    }
}