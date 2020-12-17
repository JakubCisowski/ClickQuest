using ClickQuest.Heroes;
using ClickQuest.Items;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ClickQuest.Account
{
	public partial class User : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion INotifyPropertyChanged

		#region Singleton

		private static User _instance;

		public static User Instance
		{
			get
			{
				if (_instance is null)
				{
					_instance = new User();
				}
				return _instance;
			}
			set
			{
				_instance = value;
				_instance.OnPropertyChanged();
			}
		}

		#endregion Singleton

		#region Private Fields

		private List<Hero> _heroes;
		private Hero _currentHero;
		private List<Material> _materials;
		private List<Recipe> _recipes;
		private List<Artifact> _artifacts;
		private List<Ingot> _ingots;
		private List<Blessing> _blessings;
		private int _gold;
		private Specialization _specialization;

		#endregion Private Fields

		#region Properties

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public List<Hero> Heroes
		{
			get
			{
				return _heroes;
			}
			set
			{
				_heroes = value;
				OnPropertyChanged();
			}
		}

		public List<Material> Materials
		{
			get
			{
				return _materials;
			}
			set
			{
				_materials = value;
				OnPropertyChanged();
			}
		}

		public List<Recipe> Recipes
		{
			get
			{
				return _recipes;
			}
			set
			{
				_recipes = value;
				OnPropertyChanged();
			}
		}

		public List<Artifact> Artifacts
		{
			get
			{
				return _artifacts;
			}
			set
			{
				_artifacts = value;
				OnPropertyChanged();
			}
		}

		public List<Ingot> Ingots
		{
			get
			{
				return _ingots;
			}
			set
			{
				_ingots = value;
				OnPropertyChanged();
			}
		}

		public List<Blessing> Blessings
		{
			get
			{
				return _blessings;
			}
			set
			{
				_blessings = value;
				OnPropertyChanged();
			}
		}

		[NotMapped]
		public Hero CurrentHero
		{
			get
			{
				return _currentHero;
			}
			set
			{
				_currentHero = value;
				OnPropertyChanged();
			}
		}

		public int Gold
		{
			get
			{
				return _gold;
			}
			set
			{
				_gold = value;
				OnPropertyChanged();
			}
		}

		public Specialization Specialization
		{
			get
			{
				return _specialization;
			}
			set
			{
				_specialization = value;
				OnPropertyChanged();
			}
		}

		#endregion Properties

		public User()
		{
			_heroes = new List<Hero>();
			_materials = new List<Material>();
			_recipes = new List<Recipe>();
			_artifacts = new List<Artifact>();
			_ingots = new List<Ingot>();
			_blessings = new List<Blessing>();

			_specialization=Specialization.Instance;
		}

		public void AddItem(Item itemToAdd)
		{
			var type = itemToAdd.GetType();

			if (type == typeof(Recipe))
			{
				// Add to Recipes.

				foreach (var item in Recipes)
				{
					if (item.Id == itemToAdd.Id)
					{
						item.Quantity++;
						return;
					}
				}

				// If user doesn't have this item, add it.
				Recipes.Add(itemToAdd as Recipe);
				itemToAdd.Quantity++;
			}
			else if (type == typeof(Artifact))
			{
				// Add to Artifacts.

				foreach (var item in Artifacts)
				{
					if (item.Id == itemToAdd.Id)
					{
						item.Quantity++;
						return;
					}
				}

				// If user doesn't have this item, add it.
				Artifacts.Add(itemToAdd as Artifact);
				itemToAdd.Quantity++;
			}
			else if (type == typeof(Material))
			{
				// Add to Materials.

				foreach (var item in Materials)
				{
					if (item.Id == itemToAdd.Id)
					{
						item.Quantity++;
						return;
					}
				}

				// If user doesn't have this item, add it.
				Materials.Add(itemToAdd as Material);
				itemToAdd.Quantity++;
			}
		}

		public void RemoveItem(Item itemToRemove)
		{
			var type = itemToRemove.GetType();

			if (type == typeof(Recipe))
			{
				// Revmove from Recipes.

				foreach (var item in Recipes)
				{
					if (item.Id == itemToRemove.Id)
					{
						item.Quantity--;
						if (item.Quantity <= 0)
						{
							// Remove item from database.
							Entity.EntityOperations.RemoveItem(item);
						}
						return;
					}
				}
			}
			else if (type == typeof(Artifact))
			{
				// Revmove from Artifacts.

				foreach (var item in Artifacts)
				{
					if (item.Id == itemToRemove.Id)
					{
						item.Quantity--;
						if (item.Quantity <= 0)
						{
							// Remove item from database.
							Entity.EntityOperations.RemoveItem(item);
						}
						return;
					}
				}
			}
			else if (type == typeof(Material))
			{
				// Revmove from Materials.

				foreach (var item in Materials)
				{
					if (item.Id == itemToRemove.Id)
					{
						item.Quantity--;
						if (item.Quantity <= 0)
						{
							// Remove item from database.
							Entity.EntityOperations.RemoveItem(item);
						}
						return;
					}
				}
			}

			// If user doesn't have this item, don't do anything (check Item.Quantity).
		}
	}
}