using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public partial class DungeonKey : Item
	{
		public DungeonKey() : base()
		{
		}

		public override void AddAchievementProgress(int amount)
		{
			NumericAchievementType achievementType = 0;
			// Increase achievement amount.
			switch(Rarity)
			{
				case Rarity.General:
					achievementType = NumericAchievementType.GeneralDungeonKeysEarned;
					break;
				case Rarity.Fine:
					achievementType = NumericAchievementType.FineDungeonKeysEarned;
					break;
				case Rarity.Superior:
					achievementType = NumericAchievementType.SuperiorDungeonKeysEarned;
					break;
				case Rarity.Exceptional:
					achievementType = NumericAchievementType.ExceptionalDungeonKeysEarned;
					break;
				case Rarity.Mythic:
					achievementType = NumericAchievementType.MythicDungeonKeysEarned;
					break;
				case Rarity.Masterwork:
					achievementType = NumericAchievementType.MasterworkDungeonKeysEarned;
					break;
			}
			User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
		}

		public override DungeonKey CopyItem(int quantity)
		{
			var copy = new DungeonKey();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = quantity;

			return copy;
		}

		public override void AddItem()
		{
			CollectionsController.AddItemToCollection<DungeonKey>(this, User.Instance.DungeonKeys);

			this.AddAchievementProgress(1);
			Extensions.InterfaceManager.InterfaceController.RefreshEquipmentPanels();
		}
		
		public override void RemoveItem()
		{
			CollectionsController.RemoveItemFromCollection<DungeonKey>(this, User.Instance.DungeonKeys);
		}

		public static List<double> CreateRarityChancesList(int monsterHealth)
		{
			var DungeonKeyRarityChances = new List<double>();
			double EmptyLootChance = 1;
			double DungeonKeyRarity0Chance = 0;
			double DungeonKeyRarity1Chance = 0;
			double DungeonKeyRarity2Chance = 0;
			double DungeonKeyRarity3Chance = 0;
			double DungeonKeyRarity4Chance = 0;
			double DungeonKeyRarity5Chance = 0;

			// Set dungeon key drop rates.
			if (monsterHealth < 100)
			{
				EmptyLootChance = 0.05000;
				DungeonKeyRarity0Chance = 0.8000;
				DungeonKeyRarity1Chance = 0.1500;
			}
			else if (monsterHealth < 200)
			{
				EmptyLootChance = 0.05000;
				DungeonKeyRarity0Chance = 0.4500;
				DungeonKeyRarity1Chance = 0.5000;
			}

			// Set drop rates in list for randomizing algorithm.
			// Note that index 0 is for empty loot, rarities start with index 1 up to 6.
			DungeonKeyRarityChances.Add(EmptyLootChance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity0Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity1Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity2Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity3Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity4Chance);
			DungeonKeyRarityChances.Add(DungeonKeyRarity5Chance);

			return DungeonKeyRarityChances;
		}
	}
}