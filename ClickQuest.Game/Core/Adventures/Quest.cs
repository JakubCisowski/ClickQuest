using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Quests;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Threading;

namespace ClickQuest.Game.Core.Adventures
{
	public class Quest : INotifyPropertyChanged, IIdentifiable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly DispatcherTimer _timer;

		public int DbKey { get; set; }

		public bool Rare { get; set; }

		public HeroClass HeroClass { get; set; }

		public string Name { get; set; }

		public int Duration { get; set; }

		public string Description { get; set; }

		public List<QuestRewardPattern> QuestRewardPatterns { get; set; }

		public int TicksCountNumber { get; set; }

		public string TicksCountText { get; set; }

		public string RewardsDescription { get; private set; }

		public DateTime EndDate { get; set; }
		public int Id { get; set; }

		[JsonIgnore]
		public bool IsFinished
		{
			get
			{
				return TicksCountNumber <= 0;
			}
		}

		public Quest()
		{
			QuestRewardPatterns = new List<QuestRewardPattern>();

			_timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 1)
			};
			_timer.Tick += Timer_Tick;
		}

		public Quest CopyQuest()
		{
			var copy = new Quest();

			// Copy only the Database Id, not the Entity Id.
			copy.Id = Id;
			copy.Rare = Rare;
			copy.HeroClass = HeroClass;
			copy.Name = Name;
			copy.Duration = Duration;
			copy.Description = Description;
			copy.RewardsDescription = RewardsDescription;

			copy.QuestRewardPatterns = QuestRewardPatterns;

			return copy;
		}

		public void StartQuest()
		{
			// Create copy of this quest (to make doing the same quest possible on other heroes at the same time).
			var questCopy = CopyQuest();
			questCopy.EndDate = EndDate;

			// Trigger on-quest start artifacts.
			foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
			{
				artifact.ArtifactFunctionality.OnQuestStarted(questCopy);
			}

			// Replace that quest in Hero's Quests collection with the newly copied quest.
			var questsWithMatchingId = User.Instance.CurrentHero?.Quests.Where(x => x.Id == questCopy.Id).ToList();

			questsWithMatchingId.ForEach(x => User.Instance.CurrentHero?.Quests.Remove(x));
			User.Instance.CurrentHero?.Quests.Add(questCopy);

			// Set quest end date (if not yet set).
			if (questCopy.EndDate == default)
			{
				questCopy.EndDate = DateTime.Now.AddSeconds(questCopy.Duration);
			}

			// Initially set TicksCountText (for hero stats page info).
			// Reset to 'Duration', it will count from Duration to 0.
			questCopy.TicksCountNumber = (int)(questCopy.EndDate - DateTime.Now).TotalSeconds;

			if (questCopy.IsFinished)
			{
				questCopy.HandleQuestIfFinished();
			}
			else
			{
				questCopy._timer.Start();
			}

			UpdateTicksCountText(questCopy);

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		public void FinishQuest()
		{
			_timer.Stop();
			TicksCountText = "";
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Questing]++;
			AssignRewards();
			QuestController.RerollQuests();
			CombatTimerController.StartAuraTimerOnCurrentRegion();
		}

		public void UpdateAllRewardsDescription()
		{
			foreach (var reward in QuestRewardPatterns)
			{
				string itemName = "";

				switch (reward.RewardType)
				{
					case RewardType.Material:
						itemName = GameAssets.Materials.FirstOrDefault(x => x.Id == reward.Id).Name;
						break;

					case RewardType.Recipe:
						itemName = GameAssets.Recipes.FirstOrDefault(x => x.Id == reward.Id).Name;
						break;

					case RewardType.Artifact:
						itemName = GameAssets.Artifacts.FirstOrDefault(x => x.Id == reward.Id).Name;
						break;

					case RewardType.Blessing:
						itemName = GameAssets.Blessings.FirstOrDefault(x => x.Id == reward.Id).Name;
						break;

					case RewardType.Ingot:
						itemName = GameAssets.Ingots.FirstOrDefault(x => x.Id == reward.Id).Name;
						break;
				}

				RewardsDescription += $"\n - {reward.Quantity}x {itemName} ({reward.RewardType.ToString()})";
			}
		}

		private void AssignRewards()
		{
			AlertBox.Show($"Quest {Name} finished.\nRewards granted.", MessageBoxButton.OK);
			User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestsCompleted]++;

			foreach (var materialRewardPattern in QuestRewardPatterns.Where(x => x.RewardType == RewardType.Material))
			{
				var material = GameAssets.Materials.FirstOrDefault(x => x.Id == materialRewardPattern.Id);
				material.AddItem(materialRewardPattern.Quantity);
				material.AddAchievementProgress();
			}

			foreach (var artifactRewardPattern in QuestRewardPatterns.Where(x => x.RewardType == RewardType.Artifact))
			{
				var artifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == artifactRewardPattern.Id);
				artifact.AddItem(artifactRewardPattern.Quantity);
				artifact.AddAchievementProgress();
			}

			foreach (var recipeRewardPattern in QuestRewardPatterns.Where(x => x.RewardType == RewardType.Recipe))
			{
				var recipe = GameAssets.Recipes.FirstOrDefault(x => x.Id == recipeRewardPattern.Id);
				recipe.AddItem(recipeRewardPattern.Quantity);
				recipe.AddAchievementProgress();
			}

			foreach (var ingotRewardPattern in QuestRewardPatterns.Where(x => x.RewardType == RewardType.Ingot))
			{
				var ingot = GameAssets.Ingots.FirstOrDefault(x => x.Id == ingotRewardPattern.Id);
				ingot.AddItem(ingotRewardPattern.Quantity);
				ingot.AddAchievementProgress();
			}

			foreach (var blessingRewardPattern in QuestRewardPatterns.Where(x => x.RewardType == RewardType.Blessing))
			{
				Blessing.AskUserAndSwapBlessing(blessingRewardPattern.Id);
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		public void PauseTimer()
		{
			_timer.Stop();
		}

		private void UpdateTicksCountText(Quest quest)
		{
			quest.TicksCountText = $"{quest.Name}\n{quest.TicksCountNumber / 60}m {quest.TicksCountNumber % 60}s";
		}

		private void Timer_Tick(object source, EventArgs e)
		{
			TicksCountNumber--;
			UpdateTicksCountText(this);
			HandleQuestIfFinished();
		}

		private void HandleQuestIfFinished()
		{
			if (IsFinished)
			{
				FinishQuest();
				TicksCountText = "";
			}
		}
	}
}