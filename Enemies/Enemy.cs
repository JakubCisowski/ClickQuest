using System.ComponentModel;
using ClickQuest.Interfaces;

namespace ClickQuest.Enemies
{
	public abstract class Enemy : INotifyPropertyChanged, IIdentifiable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected int _currentHealth;

		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int Health { get; set; }
		public abstract int CurrentHealth { get; set; }
		public int CurrentHealthProgress { get; set; }
		public string Image { get; set; }

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
	}
}