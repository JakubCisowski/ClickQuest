using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ClickQuest.Items
{
	public partial class Item : INotifyPropertyChanged
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
		private string _name;
		private int _value;
		private Rarity _rarity;
		private int _quantity;
		private string _description;

		#endregion Private Fields

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

				// If we set quantity to 0 or lower, then remove it from user's equipment
				if (_quantity <= 0)
				{
					Account.User.Instance.CurrentHero?.Recipes.Remove(this as Recipe);
					Account.User.Instance.CurrentHero?.Materials.Remove(this as Material);
					Account.User.Instance.CurrentHero?.Artifacts.Remove(this as Artifact);
				}

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

		#endregion Properties

		public Item(int id, string name, Rarity rarity, string description, int value)
		{
			Id = id;
			Name = name;
			Rarity = rarity;
			Value = value;
			Description = description;
			Quantity = 0;
		}
	}
}