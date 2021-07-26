using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClickQuest.Interfaces;

namespace ClickQuest.Enemies
{
	public abstract class Enemy : INotifyPropertyChanged, IIdentifiable
	{
		public Enemy()
		{
			CurrentHealthProgress = 100;
		}

		public abstract Enemy CopyEnemy();

		protected int CalculateCurrentHealthProgress()
		{
			// Calculate killing progress in % (for progress bar on monster button).
			return (int) ((double) CurrentHealth / Health * 100);
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;



		#endregion INotifyPropertyChanged

		#region Private Fields

		private int _id;
		private string _name;
		private int _health;
		protected int _currentHealth;
		private int _currentHealthProgress;
		private string _image;
		private string _description;

		#endregion Private Fields

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
			}
		}

		public abstract int CurrentHealth { get; set; }

		public int CurrentHealthProgress
		{
			get
			{
				return _currentHealthProgress;
			}
			set
			{
				_currentHealthProgress = value;
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
			}
		}

		#endregion Properties
	}
}