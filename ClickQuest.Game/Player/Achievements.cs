using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using ClickQuest.Game.Extensions.CollectionsManager;
using ClickQuest.Game.Windows;

namespace ClickQuest.Game.Player
{
	public enum NumericAchievementType { ExperienceGained, GoldEarned, GoldSpent, GeneralIngotsEarned, FineIngotsEarned, SuperiorIngotsEarned, ExceptionalIngotsEarned, MythicIngotsEarned, MasterworkIngotsEarned, GeneralDungeonKeysEarned, FineDungeonKeysEarned, SuperiorDungeonKeysEarned, ExceptionalDungeonKeysEarned, MythicDungeonKeysEarned, MasterworkDungeonKeysEarned, TotalDamageDealt, CritsAmount, PoisonTicksAmount, MonstersDefeated, DungeonsCompleted, BossesDefeated, QuestsCompleted, QuestRerollsAmount, BlessingsUsed, MaterialsGained, RecipesGained, GeneralArtifactsGained, FineArtifactsGained, SuperiorArtifactsGained, ExceptionalArtifactsGained, MythicArtifactsGained, MasterworkArtifactsGained }

	public class Achievements : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[JsonIgnore]
		public ObservableDictionary<NumericAchievementType, long> NumericAchievementCollection { get; set; }

		public string AchievementCollectionString { get; set; }

		[JsonIgnore]
		public TimeSpan TotalTimePlayed { get; set; }

		public string TotalTimePlayedString { get; set; }

		public Achievements()
		{
			int amountOfNumericAchievements = Enum.GetNames(typeof(NumericAchievementType)).Length;

			NumericAchievementCollection = new ObservableDictionary<NumericAchievementType, long>();

			CollectionInitializer.InitializeDictionary(NumericAchievementCollection);
		}

		public void SerializeAchievements()
		{
			AchievementCollectionString = Serialization.SerializeData(NumericAchievementCollection);
			TotalTimePlayedString = TotalTimePlayed.ToString();
		}

		public void DeserializeAchievements()
		{
			Serialization.DeserializeData(AchievementCollectionString, NumericAchievementCollection);
			TotalTimePlayed = TimeSpan.Parse(TotalTimePlayedString);
		}

		public void IncreaseAchievementValue(NumericAchievementType achievementType, long value)
		{
			NumericAchievementCollection[achievementType] += value;
			AchievementsWindow.Instance.RefreshSingleAchievement(achievementType);
		}
	}
}