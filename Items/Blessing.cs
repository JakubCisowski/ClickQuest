using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System;

namespace ClickQuest.Items
{
    public enum BlessingType
    {
        ClickDamage=0
    }

	public partial class Blessing : INotifyPropertyChanged
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
        private string _description;
        private Rarity _rarity;
        public BlessingType _type;
        private int _value;
        private int _duration;
        private int _buff;

        #endregion

        #region Properties

        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DbKey { get; set; }

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id=value;
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
                _name=value;
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
                _description=value;
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
                _rarity=value;
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
                _value=value;
                OnPropertyChanged();
            }
        }

        public BlessingType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type=value;
                OnPropertyChanged();
            }
        }

        public int Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration=value;
                OnPropertyChanged();
            }
        }

        public int Buff
        {
            get
            {
                return _buff;
            }
            set
            {
                _buff=value;
                OnPropertyChanged();
            }
        }

        public string TypeString
        {
            get
            {
                return Type.ToString();
            }
        }

        public string RarityString
		{
			get
			{
				string str = Rarity.ToString() + ' ';

				for (int i = 0; i < (int)Rarity; i++)
				{
					str += "✩";
				}

				return str;
			}
		}

        #endregion

        public Blessing()
        {

        }

        public Blessing(int id, string name, BlessingType type, Rarity rarity,int duration,string description, int buff, int value)
        {
            Id=id;
            Name=name;
            Type=type;
            Rarity=rarity;
            Duration=duration;
            Description=description;
            Buff=buff;
            Value=value;
        }
    }
}