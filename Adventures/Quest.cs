using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Extensions.QuestManager;
using ClickQuest.Heroes;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Adventures
{
	public class Quest : INotifyPropertyChanged, IIdentifiable
	{
		#region Private Fields

		private int _id;
		private bool _rare;
		private HeroClass _heroClass;
		private string _name;
		private int _duration;
		private string _description;
		private readonly DispatcherTimer _timer;
		private DateTime _endDate;
		private int _ticksCountNumber;
		private string _ticksCountText;

		#endregion

		#region Properties

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int DbKey { get; set; }

		public int Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
				
			}
		}

		[NotMapped]
		public bool Rare
		{
			get
			{
				return _rare;
			}
			set
			{
				_rare = value;
				
			}
		}

		[NotMapped]
		public HeroClass HeroClass
		{
			get
			{
				return _heroClass;
			}
			set
			{
				_heroClass = value;
				
			}
		}

		[NotMapped]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
				
			}
		}

		[NotMapped]
		public int Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
				
			}
		}

		[NotMapped]
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
				
			}
		}

		[NotMapped]
		public List<int> RewardRecipeIds { get; set; }

		[NotMapped]
		public List<int> RewardMaterialIds { get; set; }

		[NotMapped]
		public List<int> RewardBlessingIds { get; set; }

		[NotMapped]
		public List<int> RewardIngotIds { get; set; }

		public DateTime EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				_endDate = value;
				
			}
		}

		[NotMapped]
		public int TicksCountNumber
		{
			get
			{
				return _ticksCountNumber;
			}
			set
			{
				_ticksCountNumber = value;
				
			}
		}

		[NotMapped]
		public string TicksCountText
		{
			get
			{
				return _ticksCountText;
			}
			set
			{
				_ticksCountText = value;
				
			}
		}

		[NotMapped]
		public string RewardsDescription { get; private set; }

		public bool IsFinished
		{
			get
			{
				return TicksCountNumber <= 0;
			}
		}

		#endregion

		public Quest()
		{
			RewardRecipeIds = new List<int>();
			RewardMaterialIds = new List<int>();
			RewardBlessingIds = new List<int>();
			RewardIngotIds = new List<int>();

			_timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1)};
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

		public void UpdateAllRewardsDescription()
		{
			RewardsDescription = "Rewards:";

			UpdateSpecificRewardsDescription(RewardBlessingIds, GameData.Blessings);
			UpdateSpecificRewardsDescription(RewardRecipeIds, GameData.Recipes);
			UpdateSpecificRewardsDescription(RewardMaterialIds, GameData.Materials);
			UpdateSpecificRewardsDescription(RewardIngotIds, GameData.Ingots);
		}

		private void UpdateSpecificRewardsDescription<T>(List<int> questRewardIdsCollection, List<T> rewardsGameDataCollection) where T : IIdentifiable
		{
			var rewardIdAndCountPairs = questRewardIdsCollection.GroupBy(id => id).Select(g => new {Value = g.Key, Count = g.Count()});

			foreach (var rewardGroup in rewardIdAndCountPairs)
			{
				RewardsDescription += $"\n{rewardGroup.Count}x {rewardsGameDataCollection.FirstOrDefault(x => x.Id == rewardGroup.Value).Name}";
			}
		}

		public void StartQuest()
		{
			// Create copy of this quest (to make doing the same quest possible on other heroes at the same time).
			var questCopy = CopyQuest();
			questCopy.EndDate = EndDate;

			// Replace that quest in Hero's Quests collection with the newly copied quest.
			User.Instance.CurrentHero?.Quests.RemoveAll(x => x.Id == questCopy.Id);
			User.Instance.CurrentHero?.Quests.Add(questCopy);

			// Set quest end date (if not yet set).
			if (questCopy.EndDate == default)
			{
				questCopy.EndDate = DateTime.Now.AddSeconds(Duration);
			}

			// Initially set TicksCountText (for hero stats page info).
			// Reset to 'Duration', it will count from Duration to 0.
			questCopy.TicksCountNumber = (int) (questCopy.EndDate - DateTime.Now).TotalSeconds;

			UpdateTicksCountText(questCopy);

			// Start timer (to check if quest is finished during next tick).
			questCopy._timer.Start();

			InterfaceController.RefreshStatPanels();
		}

		public void FinishQuest()
		{
			_timer.Stop();
			TicksCountText = "";
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Questing]++;
			AssignRewards();
			QuestController.RerollQuests();
			CombatController.StartAuraTimerOnCurrentRegion();
		}

		public void PauseTimer()
		{
			_timer.Stop();
		}

		private void UpdateTicksCountText(Quest quest)
		{
			quest.TicksCountText = $"{quest.Name}\n{quest.TicksCountNumber / 60}m {quest.TicksCountNumber % 60}s";
		}

		private void AssignRewards()
		{
			AlertBox.Show($"Quest {Name} finished.\nRewards granted.", MessageBoxButton.OK);
			User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestsCompleted]++;

			GrantSpecificReward(RewardMaterialIds, GameData.Materials);
			GrantSpecificReward(RewardRecipeIds, GameData.Recipes);
			GrantSpecificReward(RewardIngotIds, GameData.Ingots);

			foreach (int blessingId in RewardBlessingIds)
			{
				Blessing.AskUserAndSwapBlessing(blessingId);
			}

			InterfaceController.RefreshEquipmentPanels();
		}

		private void GrantSpecificReward<T>(List<int> questRewardIdsCollection, List<T> rewardsGameDataCollection) where T : Item
		{
			foreach (int id in questRewardIdsCollection)
			{
				var item = rewardsGameDataCollection.FirstOrDefault(x => x.Id == id);
				item.AddItem();
				item.AddAchievementProgress(1);
			}
		}


		private void Timer_Tick(object source, EventArgs e)
		{
			TicksCountNumber--;
			UpdateTicksCountText(this);

			if (IsFinished)
			{
				FinishQuest();
				TicksCountText = "";
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		

		#endregion

		

		
	}
}