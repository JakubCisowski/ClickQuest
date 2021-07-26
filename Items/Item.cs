using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClickQuest.Interfaces;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public abstract class Item : INotifyPropertyChanged, IIdentifiable
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private int _quantity;

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DbKey { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public int Value { get; set; }

		public Rarity Rarity { get; set; }
		public string Description { get; set; }

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
					User.Instance.CurrentHero?.Recipes.Remove(this as Recipe);
					User.Instance.CurrentHero?.Materials.Remove(this as Material);
					User.Instance.CurrentHero?.Artifacts.Remove(this as Artifact);
				}
			}
		}

		public string RarityString
		{
			get
			{
				return Rarity.ToString();
			}
		}

		public abstract Item CopyItem(int quantity);
		public abstract void AddAchievementProgress(int amount);

		public abstract void AddItem();
		public abstract void RemoveItem();
	}
}