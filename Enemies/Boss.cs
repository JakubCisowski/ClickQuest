using System.Collections.Generic;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Enemies
{
	public class Boss : Enemy
	{
		private List<BossLootPattern> _bossLoot;

		public override int CurrentHealth
		{
			get { return _currentHealth; }
			set
			{
				// value - new current health
				if (value == Health)
				{
					_currentHealth = value;
				}
				else if (value <= 0)
				{
					User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth);
					_currentHealth = 0;
					User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.BossesDefeated, 1);
				}
				else
				{
					User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.TotalDamageDealt, _currentHealth - value);
					_currentHealth = value;
				}

				CurrentHealthProgress = CalculateCurrentHealthProgress();
				OnPropertyChanged();
			}
		}

		public List<BossLootPattern> BossLoot
		{
			get { return _bossLoot; }
			set
			{
				_bossLoot = value;
				OnPropertyChanged();
			}
		}

		public override Boss CopyEnemy()
		{
			var copy = new Boss();

			copy.Id = Id;
			copy.Name = Name;
			copy.Health = Health;
			copy.CurrentHealth = Health;
			copy.Description = Description;
			copy.CurrentHealthProgress = CurrentHealthProgress;
			copy.BossLoot = BossLoot;

			return copy;
		}
	}
}