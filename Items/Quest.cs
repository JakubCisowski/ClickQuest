using ClickQuest.Account;
using ClickQuest.Data;
using ClickQuest.Heroes;
using ClickQuest.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
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

			RewardRecipeIds = quest.RewardRecipeIds;
			RewardMaterialIds = quest.RewardMaterialIds;
			RewardBlessingIds = quest.RewardBlessingIds;
			RewardIngots = quest.RewardIngots;
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
				questCopy.TicksCountText = $"{questCopy.TicksCountNumber / 60}m {questCopy.TicksCountNumber % 60 + 1}s";
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

			// Refresh all stats panel bindings.
			foreach (var page in Database.Pages.Skip(2))
			{
				dynamic p = page.Value;
				p.StatsFrame.Refresh();
			}

			// Reroll new set of 3 quests.
			(Database.Pages["QuestMenu"] as QuestMenuPage).RerollQuests();
		}

		public void PauseTimer()
		{
			_timer.Stop();
		}

		private void AssignRewards()
		{
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
			}

			// Start blessings.
			foreach (var blessingId in RewardBlessingIds)
			{
				// Select right blessing.
				var blessingBlueprint = Database.Blessings.FirstOrDefault(x => x.Id == blessingId);
				// Create a new Blessing.
				var blessing = new Blessing(blessingBlueprint);
				// Increase his duration based on Blessing Specialization buff.
				blessing.Duration += User.Instance.CurrentHero.Specialization.SpecBlessingBuff;
				User.Instance.CurrentHero.Blessings.Add(blessing);
				blessing.ChangeBuffStatus(true);
			}

			// Refresh all equipment pages (skip 2 pages - MainMenu and HeroCreation, because they don't have an EquipmentFrame).
			// Alternative to .Skip(2) - try catch and continue the loop if an exception is caught (that is, if EquipmnetFrame does not exist).
			foreach (var page in Database.Pages.Skip(2))
			{
				dynamic p = page.Value;
				p.EquipmentFrame.Refresh();
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
				TicksCountText = $"{TicksCountNumber / 60}m {TicksCountNumber % 60 + 1}s";
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