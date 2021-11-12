using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Data;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Extensions.Gameplay
{
	public static class GameController
	{
		public static void OnHeroExit()
		{
			User.Instance.CurrentHero?.UpdateTimePlayed();
			User.Instance.CurrentHero?.PauseBlessing();
			User.Instance.CurrentHero?.UnequipArtfacts();
		}

		public static void ResetAllProgress()
		{
			User.Instance.LastHeroId = 0;
			User.Instance.Heroes.Clear();
			User.Instance.Gold = 0;

			User.Instance.DungeonKeys.Clear();
			UserDataLoader.SeedDungeonKeys();

			User.Instance.Ingots.Clear();
			UserDataLoader.SeedIngots();

			User.Instance.Achievements.TotalTimePlayed = default;
			User.Instance.Achievements.NumericAchievementCollection = new ObservableDictionary<NumericAchievementType, long>();
			CollectionInitializer.InitializeDictionary(User.Instance.Achievements.NumericAchievementCollection);
		}

		public static void UpdateSpecializationAmountAndUI(SpecializationType specializationType, int amountIncreased = 1)
		{
			if (User.Instance.CurrentHero is not null)
			{
				User.Instance.CurrentHero.Specialization.SpecializationAmounts[specializationType] += amountIncreased;
				
				InterfaceController.UpdateSingleSpecializationInterface(specializationType);
			}
		}
	}
}