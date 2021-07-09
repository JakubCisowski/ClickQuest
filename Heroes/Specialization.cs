using ClickQuest.Heroes;
using ClickQuest.Items;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;
using ClickQuest.Extensions;

namespace ClickQuest.Heroes
{
	public enum SpecializationType
	{
		Blessing, Clicking, Crafting, Buying, Melting, Questing, Dungeon
	}
	
	[Owned]
	public partial class Specialization : INotifyPropertyChanged
	{
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		[NotMapped]
		public ObservableDictionary<SpecializationType, int> SpecializationBuffs { get; set; }

		[NotMapped]
		public ObservableDictionary<SpecializationType, int> SpecializationThresholds { get; set; }

		[NotMapped]
		public ObservableDictionary<SpecializationType, int> SpecializationAmounts { get; set; }

		public string SpecializationAmountsString { get; set; }

		private string _specCraftingText;
		public string SpecCraftingText
		{
			get
			{
				return _specCraftingText;
			}
			set
			{
				_specCraftingText = value;
				OnPropertyChanged();
			}
		}


		#region Constants

			// Base values for each buff.
			const int SpecCraftingBuffBase = 1;
			const int SpecBuyingBuffBase = 50;
			const int SpecDungeonBuffBase = 30;

			// Value limits for each buff.
			const int SpecCraftingBuffLimit = 5;
			const int SpecQuestingBuffLimit = 50;

			// Const buff value for reaching every threshold.
			const int SpecBlessingBuffBonus = 15; // Increases blessings duration in seconds. <Base - 0>
			const int SpecClickingBuffBonus = 1;   // Increases click damage (after effects like crit, poison are applied - const value) <Base - 0>
			const int SpecCraftingBuffBonus = 1;  // Increases crafting rarity limit. <Base - 1> <Limit - 5>
			const int SpecBuyingBuffBonus = 1;    // Increases shop offer size. <Base - 5>
			const int SpecMeltingBuffBonus = 5;   // Increases % chance to get additional ingots when melting. <Base - 0%>
			const int SpecQuestingBuffBonus = 5;  // Reduces % time required to complete questes. <Base - 0%> <Limit - 50%>
			const int SpecDungeonBuffBonus = 1;   // Increases amount of time to defeat dungeon boss in seconds <Base - 30s>

		#endregion

		public Specialization()
		{
			SpecializationBuffs = new ObservableDictionary<SpecializationType, int>();
			SpecializationThresholds = new ObservableDictionary<SpecializationType, int>();
			SpecializationAmounts = new ObservableDictionary<SpecializationType, int>();

			CollectionInitializer.InitializeDictionary<SpecializationType, int>(SpecializationBuffs);
			CollectionInitializer.InitializeDictionary<SpecializationType, int>(SpecializationThresholds);
			CollectionInitializer.InitializeDictionary<SpecializationType, int>(SpecializationAmounts);

			UpdateThresholds();

			UpdateBuffs();
		}

		public void UpdateThresholds()
		{
			// Buff gains thresholds.
			SpecializationThresholds[SpecializationType.Blessing] = 10; // Amount increases every time a Blessing is bought.
			SpecializationThresholds[SpecializationType.Clicking] = 10; // Amount increases every time user clicks on monster or boss.
			SpecializationThresholds[SpecializationType.Crafting] = 10; // Amount increases every time an artifact is crafted using recipe.
			SpecializationThresholds[SpecializationType.Buying] = 10;   // Amount increases every time a Recipe is bought.
			SpecializationThresholds[SpecializationType.Melting] = 10;  // Amount increases every time a material is melted.
			SpecializationThresholds[SpecializationType.Questing] = 10; // Amount increases every time a quest is completed.
			SpecializationThresholds[SpecializationType.Dungeon] = 10;  // Amount increases every time a dungeon is finished.

			// Changes that depend on hero class.
			// Changing thresholds is easier to balance than changing buffconst.
			switch(Player.User.Instance.CurrentHero?.HeroRace)
			{
				case HeroRace.Human:
					SpecializationThresholds[SpecializationType.Crafting] = 5;
					SpecializationThresholds[SpecializationType.Buying] = 5;

					break;

				case HeroRace.Elf:
					SpecializationThresholds[SpecializationType.Questing] = 5;
					SpecializationThresholds[SpecializationType.Blessing] = 5;

					break;
					
				case HeroRace.Dwarf:
					SpecializationThresholds[SpecializationType.Melting] = 5;
					SpecializationThresholds[SpecializationType.Dungeon] = 5;

					break;
			}
		}

		public void UpdateBuffs()
		{
			// Updating current buff values based on constants and amount (which is not constant).

			SpecializationBuffs[SpecializationType.Blessing] = (SpecializationAmounts[SpecializationType.Blessing] / SpecializationThresholds[SpecializationType.Blessing]) * SpecBlessingBuffBonus;

			SpecializationBuffs[SpecializationType.Clicking] = (SpecializationAmounts[SpecializationType.Clicking] / SpecializationThresholds[SpecializationType.Clicking]) * SpecClickingBuffBonus;

			SpecializationBuffs[SpecializationType.Crafting] = Math.Min(SpecCraftingBuffBase + (SpecializationAmounts[SpecializationType.Crafting] / SpecializationThresholds[SpecializationType.Crafting]) * SpecCraftingBuffBonus, SpecCraftingBuffLimit);
			
			SpecializationBuffs[SpecializationType.Buying] = SpecBuyingBuffBase + (SpecializationAmounts[SpecializationType.Buying] / SpecializationThresholds[SpecializationType.Buying]) * SpecBuyingBuffBonus;

			SpecializationBuffs[SpecializationType.Melting] = (SpecializationAmounts[SpecializationType.Melting] / SpecializationThresholds[SpecializationType.Melting]) * SpecMeltingBuffBonus;

			SpecializationBuffs[SpecializationType.Questing] = Math.Min((SpecializationAmounts[SpecializationType.Questing] / SpecializationThresholds[SpecializationType.Questing]) * SpecQuestingBuffBonus, SpecQuestingBuffLimit);
			
			SpecializationBuffs[SpecializationType.Dungeon] = SpecDungeonBuffBase + (SpecializationAmounts[SpecializationType.Dungeon] / SpecializationThresholds[SpecializationType.Dungeon]) * SpecDungeonBuffBonus;
			
			// Update crafting text (used in hero stats panel).
			SpecCraftingText = ((Rarity)SpecializationBuffs[SpecializationType.Crafting]).ToString();
		}

		public void SerializeSpecializationAmounts()
		{
			SpecializationAmountsString = Serialization.SerializeData<SpecializationType, int>(SpecializationAmounts);
		}

		public void DeserializeSpecializationAmounts()
		{
			Serialization.DeserializeData<SpecializationType, int>(SpecializationAmountsString, SpecializationAmounts);	
		}
	}
}