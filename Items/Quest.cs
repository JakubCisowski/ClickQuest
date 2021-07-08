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

namespace ClickQuest.Items
{
	public partial class Quest : INotifyPropertyChanged
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
		#endregion

		public void CopyQuest(Quest quest)
		{
			// Copy only the Database Id, not the Entity Id.
			Id = quest.Id;
			Rare = quest.Rare;
			HeroClass = quest.HeroClass;
			Name = quest.Name;
			Duration = quest.Duration;
			Description = quest.Description;
			RewardsDescription = quest.RewardsDescription;

			RewardRecipeIds = quest.RewardRecipeIds;
			RewardMaterialIds = quest.RewardMaterialIds;
			RewardBlessingIds = quest.RewardBlessingIds;
			RewardIngots = quest.RewardIngots;
		}

		public void UpdateRewardsDescription()
		{
			RewardsDescription = "Rewards:";
			
			#region Blessings

			var counter = 1;
			var previousId = 0;

			for (int i = 0; i < RewardBlessingIds.Count; i++)
			{
				// If reward id stays the same (or it's first id in the list) - increment the counter.
				if ((i == 0 || previousId == RewardBlessingIds[i]) && (i != RewardBlessingIds.Count - 1))
				{
					counter++;
				}
				// New reward id / last id in the list - display reward info on the button.
				else
				{
					RewardsDescription += $"\n{counter}x {Database.Blessings.FirstOrDefault(x => x.Id == RewardBlessingIds[i]).Name}";
					counter = 1;
				}

				previousId = RewardBlessingIds[i];
			}

			#endregion Blessings

			#region Materials

			counter = 1;
			previousId = 0;

			for (int i = 0; i < RewardMaterialIds.Count; i++)
			{
				// If reward id stays the same (or it's first id in the list) - increment the counter.
				if ((i == 0 || previousId == RewardMaterialIds[i]) && (i != RewardMaterialIds.Count - 1))
				{
					counter++;
				}
				// New reward id / last id in the list - display reward info on the button.
				else
				{
					RewardsDescription += $"\n{counter}x {Database.Materials.FirstOrDefault(x => x.Id == RewardMaterialIds[i]).Name}";
					counter = 1;
				}

				previousId = RewardMaterialIds[i];
			}

			#endregion Materials

			#region Recipes

			counter = 1;
			previousId = 0;

			for (int i = 0; i < RewardRecipeIds.Count; i++)
			{
				// If reward id stays the same (or it's first id in the list) - increment the counter.
				if ((i == 0 || previousId == RewardRecipeIds[i]) && (i != RewardRecipeIds.Count - 1))
				{
					counter++;
				}
				// New reward id / last id in the list - display reward info on the button.
				else
				{
					RewardsDescription += $"\n{counter}x {Database.Recipes.FirstOrDefault(x => x.Id == RewardRecipeIds[i]).Name}";
					counter = 1;
				}

				previousId = RewardRecipeIds[i];
			}

			#endregion Recipes

			#region Ingots

			counter = 1;
			previousId = 0;

			for (int i = 0; i < RewardIngots.Count; i++)
			{
				// If reward id stays the same (or it's first id in the list) - increment the counter.
				if ((i == 0 || previousId == (int)RewardIngots[i]) && (i != RewardIngots.Count - 1))
				{
					counter++;
				}
				// New reward id / last id in the list - display reward info on the button.
				else
				{
					RewardsDescription += counter > 1 ? $"\n{counter}x {RewardIngots[i].ToString()} Ingots" : $"\n{counter}x {RewardIngots[i].ToString()} Ingot";
					counter = 1;
				}

				previousId = (int)RewardIngots[i];
			}

			#endregion Ingots
		}

		public void StartQuest()
		{
			// Create copy of this quest (to make doing the same quest possible on other heroes at the same time).
			var questCopy = new Quest
			{
				EndDate = this.EndDate
			};
			questCopy.CopyQuest(this);

			// Change that quest in Hero's Quests collection to the newly copied quest.
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

			// If quest is already finished
			if (questCopy.TicksCountNumber <= 0)
			{
				questCopy.TicksCountText = "";
			}
			else
			{
				// Convert to text.
				questCopy.TicksCountText = $"{questCopy.Name}\n{questCopy.TicksCountNumber / 60}m {questCopy.TicksCountNumber % 60 + 1}s";
			}

			// Start timer (checks if quest is finished).
			questCopy._timer.Start();

			// Refresh hero stats panel (for timer).
			(Database.Pages["QuestMenu"] as QuestMenuPage).StatsFrame.Refresh();
		}

		public void StopQuest()
		{
			// Stop timer.
			_timer.Stop();

			// Set TicksCountText to empty string so that it stops displaying.
			TicksCountText = "";

			// Assign rewards.
			AssignRewards();

			// Reroll new set of 3 quests.
			(Database.Pages["QuestMenu"] as QuestMenuPage).RerollQuests();

			// Start AuraTimer if user is on RegionPage.
			if ((Application.Current.MainWindow as GameWindow).CurrentFrame.Content is RegionPage regionPage)
			{
				foreach (var ctrl in regionPage.RegionPanel.Children)
				{
					if (ctrl is MonsterButton monsterButton)
					{
						monsterButton.StartAuraTimer();
						break;
					}
				}
			}
		}

		public void PauseTimer()
		{
			_timer.Stop();
		}

		private void AssignRewards()
		{
			// Inform the user about rewards.
			AlertBox.Show($"Quest {this.Name} finished.\nRewards granted.", MessageBoxButton.OK);

			// Increase achievement amount.
			User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.QuestsCompleted]++;

			// Assign materials.
			foreach (var materialId in RewardMaterialIds)
			{
				var material = Database.Materials.FirstOrDefault(x => x.Id == materialId);
				User.Instance.CurrentHero.AddItem(material);
			}

			// Assign recipes.
			foreach (var recipeId in RewardRecipeIds)
			{
				var recipe = Database.Recipes.FirstOrDefault(x => x.Id == recipeId);
				User.Instance.CurrentHero.AddItem(recipe);
			}

			// Assign ingots.
			foreach (var ingotRarity in RewardIngots)
			{
				var ingot = User.Instance.Ingots.FirstOrDefault(x => x.Rarity == ingotRarity);
				ingot.Quantity++;

				// Increase achievement amount.
				switch(ingotRarity)
				{
					case Rarity.General:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.GeneralIngotsEarned]++;
						break;
					case Rarity.Fine:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.FineIngotsEarned]++; 
						break;
					case Rarity.Superior:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.SuperiorIngotsEarned]++;
						break;
					case Rarity.Exceptional:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.ExceptionalIngotsEarned]++;
						break;
					case Rarity.Mythic:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MythicIngotsEarned]++;
						break;
					case Rarity.Masterwork:
						User.Instance.Achievements.NumericAchievementCollection[NumericAchievementType.MasterworkIngotsEarned]++;
						break;
				}
				AchievementsWindow.Instance.UpdateAchievements();
			}

			// Start blessings.
			foreach (var blessingId in RewardBlessingIds)
			{
				// Select right blessing.
				var blessingBlueprint = Database.Blessings.FirstOrDefault(x => x.Id == blessingId);

				MessageBoxResult result;

				// If there are any blessings active, ask if user wants to swap.
				if (User.Instance.CurrentHero.Blessings.Any())
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
					var blessing = new Blessing(blessingBlueprint);
					// Increase his duration based on Blessing Specialization buff.
					blessing.Duration += User.Instance.CurrentHero.Specialization.SpecBlessingBuff;
					User.Instance.CurrentHero.Blessings.Add(blessing);
					blessing.ChangeBuffStatus(true);
				}
			}

			// Grant Specialization Questing progress.
			User.Instance.CurrentHero.Specialization.SpecQuestingAmount++;

			// Refresh all stats and equipment pages (skip 2 pages - MainMenu and HeroCreation, because they don't have an EquipmentFrame).
			// Alternative to .Skip(2) - try catch and continue the loop if an exception is caught (that is, if EquipmentFrame does not exist).
			foreach (var page in Database.Pages.Skip(2))
			{
				dynamic p = page.Value;
				p.EquipmentFrame.Refresh();
				p.StatsFrame.Refresh();
			}
		}

		private void Timer_Tick(object source, EventArgs e)
		{
			// Check if quest is finished.
			if (TicksCountNumber <= 0)
			{
				StopQuest();
			}
			else
			{
				// Else, decrement TicksCountNumber, and convert it to text.
				TicksCountNumber--;
				TicksCountText = $"{Name}\n{TicksCountNumber / 60}m {TicksCountNumber % 60 + 1}s";
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

		public Quest(int id, bool rare, HeroClass heroClass, string name, int duration, string description)
		{
			Id = id;
			Rare = rare;
			HeroClass = heroClass;
			Name = name;
			Duration = duration;
			Description = description;

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