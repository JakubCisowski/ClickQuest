using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Windows;
using System.Collections.Generic;
using System.Windows;

namespace ClickQuest.Game.Core.Items
{
    public class DungeonKey : Item
    {
        public override void AddAchievementProgress(int amount = 1)
        {
            NumericAchievementType achievementType = 0;
            // Increase achievement amount.
            switch (Rarity)
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
            DungeonKey copy = new DungeonKey
            {
                Id = Id,
                Name = Name,
                Rarity = Rarity,
                Value = Value,
                Description = Description,
                Quantity = quantity
            };

            return copy;
        }

        public override void AddItem(int amount = 1)
        {
            CollectionsController.AddItemToCollection(this, User.Instance.DungeonKeys, amount);

            if (amount != 0)
            {
                (Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"+{amount}", ColorsController.GetRarityColor(Rarity), FloatingTextController.GetDungeonKeyRarityPosition(Rarity));
            }

            AddAchievementProgress();
            InterfaceController.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(DungeonKey));
        }

        public override void RemoveItem(int amount = 1)
        {
            CollectionsController.RemoveItemFromCollection(this, User.Instance.DungeonKeys, amount);

            if (amount != 0)
            {
                (Application.Current.MainWindow as GameWindow).CreateFloatingTextUtility($"-{amount}", ColorsController.GetRarityColor(Rarity), FloatingTextController.GetDungeonKeyRarityPosition(Rarity));
            }

            InterfaceController.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(DungeonKey));
        }

        public static List<double> CreateRarityChancesList(int monsterHealth)
        {
            var dungeonKeyRarityChances = new List<double>();
            double emptyLootChance = 1;
            double dungeonKeyRarity0Chance = 0;
            double dungeonKeyRarity1Chance = 0;
            double dungeonKeyRarity2Chance = 0;
            double dungeonKeyRarity3Chance = 0;
            double dungeonKeyRarity4Chance = 0;
            double dungeonKeyRarity5Chance = 0;

            // Set dungeon key drop rates.
            if (monsterHealth < 100)
            {
                emptyLootChance = 0.05000;
                dungeonKeyRarity0Chance = 0.8000;
                dungeonKeyRarity1Chance = 0.1500;
            }
            else if (monsterHealth < 200)
            {
                emptyLootChance = 0.05000;
                dungeonKeyRarity0Chance = 0.4500;
                dungeonKeyRarity1Chance = 0.5000;
            }

            // Set drop rates in list for randomizing algorithm.
            // Note that index 0 is for empty loot, rarities start with index 1 up to 6.
            dungeonKeyRarityChances.Add(emptyLootChance);
            dungeonKeyRarityChances.Add(dungeonKeyRarity0Chance);
            dungeonKeyRarityChances.Add(dungeonKeyRarity1Chance);
            dungeonKeyRarityChances.Add(dungeonKeyRarity2Chance);
            dungeonKeyRarityChances.Add(dungeonKeyRarity3Chance);
            dungeonKeyRarityChances.Add(dungeonKeyRarity4Chance);
            dungeonKeyRarityChances.Add(dungeonKeyRarity5Chance);

            return dungeonKeyRarityChances;
        }
    }
}