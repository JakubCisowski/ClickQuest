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
		private List<Rarity> _rewardIngots;
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
		public List<Rarity> RewardIngots
		{
			get
			{
				return _rewardIngots;
			}
			set
			{
				_rewardIngots = value;
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
			copy.RewardIngots = RewardIngots;

			return copy;
		}

		public void UpdateAllRewardsDescription()
		{
			RewardsDescription = "Rewards:";

			UpdateOtherRewardsDescription<Blessing>(RewardBlessingIds, GameData.Blessings);
			UpdateOtherRewardsDescription<Recipe>(RewardRecipeIds, GameData.Recipes);
			UpdateOtherRewardsDescription<Material>(RewardMaterialIds, GameData.Materials);
			UpdateIngotRewardDescription();
		}

		private void UpdateOtherRewardsDescription<T>(List<int> questRewardIdsCollection, List<T> rewardsGameDataCollection) where T : IIdentifiable
		{
			var rewardIdAndCountPairs = questRewardIdsCollection
                .GroupBy(id => id)
                .Select(g => new { Value = g.Key, Count = g.Count() });

			foreach (var rewardGroup in rewardIdAndCountPairs)
			{
				RewardsDescription += $"\n{rewardGroup.Count}x {rewardsGameDataCollection.FirstOrDefault(x => x.Id == rewardGroup.Value).Name}";
			}
		}

		private void UpdateIngotRewardDescription()
		{
			var rewardIdAndCountPairs = RewardIngots
                .GroupBy(rarity => rarity)
                .Select(g => new { Value = g.Key, Count = g.Count() });

			foreach (var rewardGroup in rewardIdAndCountPairs)
			{
				RewardsDescription += rewardGroup.Count > 1 ? $"\n{rewardGroup.Count}x {rewardGroup.Value.ToString()} Ingots" : $"\n{rewardGroup.Count}x {rewardGroup.Value.ToString()} Ingot";
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
			if (quest.IsFinished)
			{
				quest.TicksCountText = "";
			}
			else
			{
				quest.TicksCountText = $"{quest.Name}\n{quest.TicksCountNumber / 60}m {quest.TicksCountNumber % 60 + 1}s";
			}
		}

		private void AssignRewards()
		{
			AlertBox.Show($"Quest {this.Name} finished.\nRewards granted.", MessageBoxButton.OK);
			User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestsCompleted]++;

			// Assign materials.
			foreach (var materialId in RewardMaterialIds)
			{
				var material = GameData.Materials.FirstOrDefault(x => x.Id == materialId);
				User.Instance.CurrentHero.AddItem(material);
			}

			// Assign recipes.
			foreach (var recipeId in RewardRecipeIds)
			{
				var recipe = GameData.Recipes.FirstOrDefault(x => x.Id == recipeId);
				User.Instance.CurrentHero.AddItem(recipe);
			}

			// Assign ingots.
			foreach (var ingotRarity in RewardIngots)
			{
				var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == ingotRarity);
				ingot.Quantity++;

				NumericAchievementType achievementType = 0;
				// Increase achievement amount.
				switch(ingotRarity)
				{
					case Rarity.General:
						achievementType = NumericAchievementType.GeneralIngotsEarned;
						break;
					case Rarity.Fine:
						achievementType = NumericAchievementType.FineIngotsEarned;
						break;
					case Rarity.Superior:
						achievementType = NumericAchievementType.SuperiorIngotsEarned;
						break;
					case Rarity.Exceptional:
						achievementType = NumericAchievementType.ExceptionalIngotsEarned;
						break;
					case Rarity.Mythic:
						achievementType = NumericAchievementType.MythicIngotsEarned;
						break;
					case Rarity.Masterwork:
						achievementType = NumericAchievementType.MasterworkIngotsEarned;
						break;
				}
				User.Instance.Achievements.IncreaseAchievementValue(achievementType, 1);
			}

			// Start blessings.
			foreach (var blessingId in RewardBlessingIds)
			{
				// Select right blessing.
				var blessingBlueprint = GameData.Blessings.FirstOrDefault(x => x.Id == blessingId);

				MessageBoxResult result;

				// If there are any blessings active, ask if user wants to swap.
				if (User.Instance.CurrentHero.Blessing != null)
				{
					// Ask user if he wants to swap current blessing.
					result = AlertBox.Show($"Do you want to swap current blessing to {blessingBlueprint.Name}?\n{blessingBlueprint.Description}",MessageBoxButton.YesNo);
				}
				else
				{
					// Else, set default option to yes.
					result = MessageBoxResult.OK;
				}
				
				// If user wants to change his blessing to a new one.
				if(result == MessageBoxResult.OK)
				{
					// Create a new Blessing.
					var blessing = blessingBlueprint.CopyBlessing();
					// Increase his duration based on Blessing Specialization buff.
					blessing.Duration += User.Instance.CurrentHero.Specialization.SpecializationBuffs[SpecializationType.Blessing];
					User.Instance.CurrentHero.Blessing = blessing;
					blessing.EnableBuff();
				}
			}

			// Grant Specialization Questing progress.
			User.Instance.CurrentHero.Specialization.SpecializationAmounts[SpecializationType.Questing]++;

			// Refresh all stats and equipment pages (skip 2 pages - MainMenu and HeroCreation, because they don't have an EquipmentFrame).
			// Alternative to .Skip(2) - try catch and continue the loop if an exception is caught (that is, if EquipmentFrame does not exist).
			foreach (var page in GameData.Pages.Skip(2))
			{
				dynamic p = page.Value;
				p.EquipmentFrame.Refresh();
				p.StatsFrame.Refresh();
			}
		}

		private void Timer_Tick(object source, EventArgs e)
		{
			if (IsFinished)
			{
				FinishQuest();
			}
			else
			{
				TicksCountNumber--;
				UpdateTicksCountText(this);
			}
		}

		public Quest()
		{
			RewardRecipeIds = new List<int>();
			RewardMaterialIds = new List<int>();
			RewardBlessingIds = new List<int>();
			RewardIngots = new List<Rarity>();

			_timer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 1)
			};
			_timer.Tick += Timer_Tick;
		}
	}
}