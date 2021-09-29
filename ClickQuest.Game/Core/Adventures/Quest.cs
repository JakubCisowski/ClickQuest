using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.Interface;
using ClickQuest.Game.Extensions.Quests;
using ClickQuest.Game.UserInterface.Controls;

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

		public List<int> RewardRecipeIds { get; set; }

		public List<int> RewardMaterialIds { get; set; }

		public List<int> RewardBlessingIds { get; set; }

		public List<int> RewardIngotIds { get; set; }

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
			RewardRecipeIds = new List<int>();
			RewardMaterialIds = new List<int>();
			RewardBlessingIds = new List<int>();
			RewardIngotIds = new List<int>();

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

			copy.RewardRecipeIds = RewardRecipeIds;
			copy.RewardMaterialIds = RewardMaterialIds;
			copy.RewardBlessingIds = RewardBlessingIds;
			copy.RewardIngotIds = RewardIngotIds;

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
			UpdateSpecificRewardsDescription(RewardBlessingIds, GameAssets.Blessings);
			UpdateSpecificRewardsDescription(RewardRecipeIds, GameAssets.Recipes);
			UpdateSpecificRewardsDescription(RewardMaterialIds, GameAssets.Materials);
			UpdateSpecificRewardsDescription(RewardIngotIds, GameAssets.Ingots);
		}

		private void UpdateSpecificRewardsDescription<T>(List<int> questRewardIdsCollection, List<T> rewardsGameAssetsCollection) where T : IIdentifiable
		{
			var rewardIdAndCountPairs = questRewardIdsCollection.GroupBy(id => id).Select(g => new
			{
				Value = g.Key,
				Count = g.Count()
			});

			foreach (var rewardGroup in rewardIdAndCountPairs)
			{
				RewardsDescription += $"\n - {rewardGroup.Count}x {rewardsGameAssetsCollection.FirstOrDefault(x => x.Id == rewardGroup.Value).Name}";
			}
		}

		private void AssignRewards()
		{
			AlertBox.Show($"Quest {Name} finished.\nRewards granted.", MessageBoxButton.OK);
			User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestsCompleted]++;

			GrantSpecificReward(RewardMaterialIds, GameAssets.Materials);
			GrantSpecificReward(RewardRecipeIds, GameAssets.Recipes);
			GrantSpecificReward(RewardIngotIds, GameAssets.Ingots);

			foreach (int blessingId in RewardBlessingIds)
			{
				Blessing.AskUserAndSwapBlessing(blessingId);
			}

			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		private void GrantSpecificReward<T>(List<int> questRewardIdsCollection, List<T> rewardsGameAssetsCollection) where T : Item
		{
			foreach (int id in questRewardIdsCollection)
			{
				var item = rewardsGameAssetsCollection.FirstOrDefault(x => x.Id == id);
				item.AddItem();
				item.AddAchievementProgress();
			}
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