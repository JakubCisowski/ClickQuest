using ClickQuest.Player;
using ClickQuest.Items;
using ClickQuest.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClickQuest.Interfaces;

namespace ClickQuest.Enemies
{
	
public abstract partial class Enemy : INotifyPropertyChanged, IIdentifiable
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
	private int _health;
	private int _currentHealth;
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

	public int Health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = value;
			OnPropertyChanged();
		}
	}

	public int CurrentHealth
	{
		get
		{
			return _currentHealth;
		}
		set
		{
			// value - new current health
			if (value == Health)
			{
				_currentHealth = value;
			}
			else if (value < 0)
			{
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth);
				_currentHealth = 0;
			}
			else
			{
				User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth - value);
				_currentHealth = value;
			}

			CurrentHealthProgress = this.CalculateCurrentHealthProgress();
			OnPropertyChanged();
		}
	}

	public int CurrentHealthProgress
	{
		get
		{
			return _currentHealthProgress;
		}
		set
		{
			_currentHealthProgress = value;
			OnPropertyChanged();
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
			OnPropertyChanged();
		}
	}

		#endregion Properties

	public abstract Enemy CopyEnemy();

	public Enemy()
	{
		CurrentHealthProgress = 100;
	}

	private int CalculateCurrentHealthProgress()
	{
		// Calculate killing progress in % (for progress bar on monster button).
		return (int)(((double)this.CurrentHealth / this.Health) * 100);
	}
}
}