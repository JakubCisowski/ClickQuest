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

			for (int i=0;i<amountOfNumericAchievements;i++)
			{
				NumericAchievementCollection.Add((NumericAchievementType)i,0);
			}
		}

		public void SerializeAchievements()
		{
			var builder = new StringBuilder();

			int i = 0;

			foreach (var pair in NumericAchievementCollection)
			{
				builder.Append($"{pair.Value.ToString()}");

				if (i != NumericAchievementCollection.Count() - 1)
				{
					builder.Append(',');
				}

				i++;
			}

			AchievementCollectionString = builder.ToString();
		}

		public void DeserializeAchievements()
		{
			if (string.IsNullOrEmpty(AchievementCollectionString))
			{
				return;
			}

			int indexOfComma = -1;
			int i = 0;

			while ((indexOfComma = AchievementCollectionString.IndexOf(',')) != -1)
			{
				string value = AchievementCollectionString.Substring(0, indexOfComma);
				AchievementCollectionString = AchievementCollectionString.Remove(0, indexOfComma+1);
				NumericAchievementCollection[(NumericAchievementType)i] = long.Parse(value);
				i++;
			}

			NumericAchievementCollection[(NumericAchievementType)i]=long.Parse(AchievementCollectionString);			
		}
	}
}
