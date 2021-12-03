using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Gameplay;
using ClickQuest.Game.Extensions.Quests;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Adventures
{
	public class Quest : INotifyPropertyChanged, IIdentifiable
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private readonly DispatcherTimer _timer;
		public static readonly int RerollGoldCost = 100;

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

		public bool IsFinished => TicksCountNumber <= 0;

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
			Quest copy = new Quest
			{
				Id = Id,
				Rare = Rare,
				HeroClass = HeroClass,
				Name = Name,
				Duration = Duration,
				Description = Description,
				RewardsDescription = RewardsDescription,
				QuestRewardPatterns = QuestRewardPatterns
			};

			// Copy only the Database Id, not the Entity Id.

			return copy;
		}

		public void StartQuest()
		{
			// Create copy of this quest (to make doing the same quest possible on other heroes at the same time).
			Quest questCopy = CopyQuest();
			questCopy.EndDate = EndDate;

			// Trigger on-quest start artifacts.
			foreach (Artifact artifact in User.Instance.CurrentHero.EquippedArtifacts)
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
			questCopy.TicksCountNumber = (int) (questCopy.EndDate - DateTime.Now).TotalSeconds;

			if (questCopy.IsFinished)
			{
				questCopy.HandleQuestIfFinished();
			}
			else
			{
				questCopy._timer.Start();
			}

			UpdateTicksCountText(questCopy);

			InterfaceController.RefreshQuestInterfaceOnCurrentPage();
		}

		public void FinishQuest()
		{
			_timer.Stop();
			TicksCountText = "";
			GameController.UpdateSpecializationAmountAndUi(SpecializationType.Questing);
			AssignRewards();
			QuestController.RerollQuests();
			CombatTimerController.StartAuraTimerOnCurrentRegion();
		}

		public void UpdateAllRewardsDescription()
		{
			foreach (QuestRewardPattern reward in QuestRewardPatterns)
			{
				var itemName = "";

				switch (reward.QuestRewardType)
				{
					case RewardType.Material:
						itemName = GameAssets.Materials.FirstOrDefault(x => x.Id == reward.QuestRewardId).Name;
						break;

					case RewardType.Recipe:
						itemName = GameAssets.Recipes.FirstOrDefault(x => x.Id == reward.QuestRewardId).Name;
						break;

					case RewardType.Artifact:
						itemName = GameAssets.Artifacts.FirstOrDefault(x => x.Id == reward.QuestRewardId).Name;
						break;

					case RewardType.Blessing:
						itemName = GameAssets.Blessings.FirstOrDefault(x => x.Id == reward.QuestRewardId).Name;
						break;

					case RewardType.Ingot:
						itemName = GameAssets.Ingots.FirstOrDefault(x => x.Id == reward.QuestRewardId).Name;
						break;
				}

				RewardsDescription += $"\n - {reward.Quantity}x {itemName} ({reward.QuestRewardType.ToString()})";
			}
		}

		private void AssignRewards()
		{
			User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestsCompleted]++;

			foreach (QuestRewardPattern materialRewardPattern in QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Material))
			{
				var material = GameAssets.Materials.FirstOrDefault(x => x.Id == materialRewardPattern.QuestRewardId);
				material.AddItem(materialRewardPattern.Quantity);
			}

			foreach (QuestRewardPattern artifactRewardPattern in QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Artifact))
			{
				var artifact = GameAssets.Artifacts.FirstOrDefault(x => x.Id == artifactRewardPattern.QuestRewardId);
				artifact.AddItem(artifactRewardPattern.Quantity);
			}

			foreach (QuestRewardPattern recipeRewardPattern in QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Recipe))
			{
				var recipe = GameAssets.Recipes.FirstOrDefault(x => x.Id == recipeRewardPattern.QuestRewardId);
				recipe.AddItem(recipeRewardPattern.Quantity);
			}

			foreach (QuestRewardPattern ingotRewardPattern in QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Ingot))
			{
				var ingot = GameAssets.Ingots.FirstOrDefault(x => x.Id == ingotRewardPattern.QuestRewardId);
				ingot.AddItem(ingotRewardPattern.Quantity);
			}

			foreach (QuestRewardPattern blessingRewardPattern in QuestRewardPatterns.Where(x => x.QuestRewardType == RewardType.Blessing))
			{
				Blessing.AskUserAndSwapBlessing(blessingRewardPattern.QuestRewardId);
			}

			InterfaceController.RefreshCurrentEquipmentPanelTabOnCurrentPage();
			InterfaceController.RefreshQuestInterfaceOnCurrentPage();
		}

		public void PauseTimer()
		{
			_timer.Stop();
		}

		private static void UpdateTicksCountText(Quest quest)
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