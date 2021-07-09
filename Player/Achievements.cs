using ClickQuest.Heroes;
using ClickQuest.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using ClickQuest.Extensions;
using System.Text;

namespace ClickQuest.Player
{
	public enum NumericAchievementType
	{
		ExperienceGained, GoldEarned, GoldSpent, GeneralIngotsEarned, FineIngotsEarned, SuperiorIngotsEarned, ExceptionalIngotsEarned, MythicIngotsEarned, MasterworkIngotsEarned, GeneralDungeonKeysEarned, FineDungeonKeysEarned, SuperiorDungeonKeysEarned, ExceptionalDungeonKeysEarned, MythicDungeonKeysEarned, MasterworkDungeonKeysEarned, TotalDamageDealt, CritsAmount, PoisonTicksAmount, MonstersDefeated, DungeonsCompleted, BossesDefeated, QuestsCompleted, QuestRerollsAmount, BlessingsUsed, MaterialsGained, RecipesGained, GeneralArtifactsGained, FineArtifactsGained, SuperiorArtifactsGained, ExceptionalArtifactsGained, MythicArtifactsGained, MasterworkArtifactsGained
	}

	[Owned]
	public partial class Achievements : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		private TimeSpan _totalTimePlayed;

		[NotMapped]
		public ObservableDictionary<NumericAchievementType, long> NumericAchievementCollection {get;set;}

		public string AchievementCollectionString { get; set; }

		public TimeSpan TotalTimePlayed
		{
			get
			{
				return _totalTimePlayed;
			}
			set
			{
				_totalTimePlayed=value;
				OnPropertyChanged();
			}
		}
	
		public Achievements()
		{
			var amountOfNumericAchievements = Enum.GetNames(typeof(NumericAchievementType)).Length;

			NumericAchievementCollection = new ObservableDictionary<NumericAchievementType, long>();

			CollectionInitializer.InitializeDictionary<NumericAchievementType, long>(NumericAchievementCollection);
		}

		public void SerializeAchievements()
		{
			AchievementCollectionString = Serialization.SerializeData<NumericAchievementType, long>(NumericAchievementCollection);
		}

		public void DeserializeAchievements()
		{
			Serialization.DeserializeData<NumericAchievementType, long>(AchievementCollectionString, NumericAchievementCollection);	
		}
	}
}
