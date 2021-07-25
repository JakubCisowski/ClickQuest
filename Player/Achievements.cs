using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Windows;
using Microsoft.EntityFrameworkCore;

namespace ClickQuest.Player
{
	public enum NumericAchievementType
	{
		ExperienceGained,
		GoldEarned,
		GoldSpent,
		GeneralIngotsEarned,
		FineIngotsEarned,
		SuperiorIngotsEarned,
		ExceptionalIngotsEarned,
		MythicIngotsEarned,
		MasterworkIngotsEarned,
		GeneralDungeonKeysEarned,
		FineDungeonKeysEarned,
		SuperiorDungeonKeysEarned,
		ExceptionalDungeonKeysEarned,
		MythicDungeonKeysEarned,
		MasterworkDungeonKeysEarned,
		TotalDamageDealt,
		CritsAmount,
		PoisonTicksAmount,
		MonstersDefeated,
		DungeonsCompleted,
		BossesDefeated,
		QuestsCompleted,
		QuestRerollsAmount,
		BlessingsUsed,
		MaterialsGained,
		RecipesGained,
		GeneralArtifactsGained,
		FineArtifactsGained,
		SuperiorArtifactsGained,
		ExceptionalArtifactsGained,
		MythicArtifactsGained,
		MasterworkArtifactsGained
	}

	[Owned]
	public class Achievements : INotifyPropertyChanged
	{
		private TimeSpan _totalTimePlayed;

		public Achievements()
		{
			int amountOfNumericAchievements = Enum.GetNames(typeof(NumericAchievementType)).Length;

			NumericAchievementCollection = new ObservableDictionary<NumericAchievementType, long>();

			CollectionInitializer.InitializeDictionary(NumericAchievementCollection);
		}

		[NotMapped]
		public ObservableDictionary<NumericAchievementType, long> NumericAchievementCollection { get; set; }

		public string AchievementCollectionString { get; set; }

		public TimeSpan TotalTimePlayed
		{
			get
			{
				return _totalTimePlayed;
			}
			set
			{
				_totalTimePlayed = value;
				OnPropertyChanged();
			}
		}

		public void SerializeAchievements()
		{
			AchievementCollectionString = Serialization.SerializeData(NumericAchievementCollection);
		}

		public void DeserializeAchievements()
		{
			Serialization.DeserializeData(AchievementCollectionString, NumericAchievementCollection);
		}

		public void IncreaseAchievementValue(NumericAchievementType achievementType, long value)
		{
			NumericAchievementCollection[achievementType] += value;
			AchievementsWindow.Instance.RefreshAchievementsPanel();
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}