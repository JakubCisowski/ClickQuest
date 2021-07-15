using ClickQuest.Player;
using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Heroes;
using ClickQuest.Pages;
using ClickQuest.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Heroes.Buffs;
using ClickQuest.Interfaces;

namespace ClickQuest.Items
{
	public partial class Quest : INotifyPropertyChanged, IIdentifiable
	{
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		#region Private Fields
		private int _id;
		private bool _rare;
		private HeroClass _heroClass;
		private string _name;
		private int _duration;
		private string _description;
		private List<int> _rewardRecipeIds;
		private List<int> _rewardMaterialIds;
		private List<int> _rewardBlessingIds;
		private List<int> _rewardIngotIds;
		private DispatcherTimer _timer;
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
				OnPropertyChanged();
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
				OnPropertyChanged();
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
				OnPropertyChanged();
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
				OnPropertyChanged();
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
				OnPropertyChanged();
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
				OnPropertyChanged();
			}
		}
		[NotMapped]
		public List<int> RewardRecipeIds
		{
			get
			{
				return _rewardRecipeIds;
			}
			set
			{
				_rewardRecipeIds = value;
			}
		}
		[NotMapped]
		public List<int> RewardMaterialIds
		{
			get
			{
				return _rewardMaterialIds;
			}
			set
			{
				_rewardMaterialIds = value;
			}
		}
		[NotMapped]
		public List<int> RewardBlessingIds
		{
			get
			{
				return _rewardBlessingIds;
			}
			set
			{
				_rewardBlessingIds = value;
			}
		}
		[NotMapped]
		public List<int> RewardIngotIds
		{
			get
			{
				return _rewardIngotIds;
			}
			set
			{
				_rewardIngotIds = value;
			}
		}
		public DateTime EndDate
		{
			get
			{
				return _endDate;
			}
			set
			{
				_endDate = value;
				OnPropertyChanged();
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
				OnPropertyChanged();
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
				OnPropertyChanged();
			}
		}
		[NotMapped]
		public string RewardsDescription { get; private set; }
		public bool IsFinished{ 
			get{
				return TicksCountNumber<=0;
			}
		}
		#endregion

		public Quest CopyQuest()
		{
			Quest copy = new Quest();

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

			UpdateSpecificRewardsDescription<Blessing>(RewardBlessingIds, GameData.Blessings);
			UpdateSpecificRewardsDescription<Recipe>(RewardRecipeIds, GameData.Recipes);
			UpdateSpecificRewardsDescription<Material>(RewardMaterialIds, GameData.Materials);
			UpdateSpecificRewardsDescription<Ingot>(RewardIngotIds, GameData.Ingots);
		}

		private void UpdateSpecificRewardsDescription<T>(List<int> questRewardIdsCollection, List<T> rewardsGameDataCollection) where T : IIdentifiable
		{
			var rewardIdAndCountPairs = questRewardIdsCollection
                .GroupBy(id => id)
                .Select(g => new { Value = g.Key, Count = g.Count() });

			foreach (var rewardGroup in rewardIdAndCountPairs)
			{
				RewardsDescription += $"\n{rewardGroup.Count}x {rewardsGameDataCollection.FirstOrDefault(x => x.Id == rewardGroup.Value).Name}";
			}
		}
		
		public void StartQuest()
		{
			// Create copy of this quest (to make doing the same quest possible on other heroes at the same time).
			var questCopy = CopyQuest();
			questCopy.EndDate = this.EndDate;

			// Replace that quest in Hero's Quests collection with the newly copied quest.
			User.Instance.CurrentHero?.Quests.RemoveAll(x => x.Id == questCopy.Id);
			User.Instance.CurrentHero?.Quests.Add(questCopy);

			// Set quest end date (if not yet set).
			if (questCopy.EndDate == default(DateTime))
			{
				questCopy.EndDate = DateTime.Now.AddSeconds(Duration);
			}

			// Initially set TicksCountText (for hero stats page info).
			// Reset to 'Duration', it will count from Duration to 0.
			questCopy.TicksCountNumber = (int)(questCopy.EndDate - DateTime.Now).TotalSeconds;

			UpdateTicksCountText(questCopy);

			// Start timer (to check if quest is finished during next tick).
			questCopy._timer.Start();

			Extensions.InterfaceManager.InterfaceController.RefreshStatPanels();
		}

		public void FinishQuest()
		{
			_timer.Stop();
			TicksCountText = "";
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Questing]++;
			AssignRewards();
			Extensions.QuestManager.QuestController.RerollQuests();
			Extensions.CombatManager.CombatController.StartAuraTimerOnEachRegion();
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
			AlertBox.Show($"Quest {this.Name} finished.\nRewards granted.", MessageBoxButton.OK);
			User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestsCompleted]++;
			
			GrantSpecificReward<Material>(RewardMaterialIds, GameData.Materials);
			GrantSpecificReward<Recipe>(RewardRecipeIds, GameData.Recipes);
			GrantSpecificReward<Ingot>(RewardIngotIds, GameData.Ingots);

			foreach (var blessingId in RewardBlessingIds)
			{
				Blessing.AskUserAndSwapBlessing(blessingId);
			}

			Extensions.InterfaceManager.InterfaceController.RefreshEquipmentPanels();
		}

		private void GrantSpecificReward<T>(List<int> questRewardIdsCollection, List<T> rewardsGameDataCollection) where T:Item
		{
			foreach (var id in questRewardIdsCollection)
			{
				var item = rewardsGameDataCollection.FirstOrDefault(x => x.Id == id);
				User.Instance.CurrentHero.AddItem(item);
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
	}
}