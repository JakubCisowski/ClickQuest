using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.UserInterface.Windows;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ClickQuest.Game.Core.Player
{
    public enum NumericAchievementType
    {
        ExperienceGained, GoldEarned, GoldSpent, GeneralIngotsEarned, FineIngotsEarned, SuperiorIngotsEarned, ExceptionalIngotsEarned,
        MythicIngotsEarned, MasterworkIngotsEarned, GeneralDungeonKeysEarned, FineDungeonKeysEarned, SuperiorDungeonKeysEarned,
        ExceptionalDungeonKeysEarned, MythicDungeonKeysEarned, MasterworkDungeonKeysEarned, TotalDamageDealt, CritsAmount, PoisonTicksAmount,
        MonstersDefeated, DungeonsCompleted, BossesDefeated, QuestsCompleted, QuestRerollsAmount, BlessingsUsed, MaterialsGained, RecipesGained,
        GeneralArtifactsGained, FineArtifactsGained, SuperiorArtifactsGained, ExceptionalArtifactsGained, MythicArtifactsGained, MasterworkArtifactsGained
    }

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

            // After deserializing, refresh the UI.
            AchievementsWindow.Instance.RefreshAchievementsPanel();
        }

        public void IncreaseAchievementValue(NumericAchievementType achievementType, long value)
        {
            NumericAchievementCollection[achievementType] += value;
            AchievementsWindow.Instance.RefreshSingleAchievement(achievementType);
        }
    }
}