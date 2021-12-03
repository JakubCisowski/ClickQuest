using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using System;
using System.ComponentModel;

namespace ClickQuest.Game.Core.Heroes.Buffs
{
    public enum SpecializationType { Blessing, Clicking, Crafting, Trading, Melting, Questing, Dungeon }

    public class Specialization : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Base SellRatio value per point of SpecTrading Buff.
        public const double SpecTradingRatioIncreasePerBuffValue = 0.01;

        // Base values for each buff.
        public const int SpecCraftingBuffBase = 1;
        public const int SpecTradingBuffBase = 5;
        public const int SpecDungeonBuffBase = 30;

        // Value limits for each buff.
        public const int SpecCraftingBuffLimit = 5;
        public const int SpecQuestingBuffLimit = 50;

        // Const buff value for reaching every threshold.
        public const int SpecBlessingBuffBonus = 15; // Increases blessings duration in seconds. <Base - 0>
        public const int SpecClickingBuffBonus = 1; // Increases click damage (after effects like crit, poison are applied - const value) <Base - 0>
        public const int SpecCraftingBuffBonus = 1; // Increases crafting rarity limit. <Base - 1> <Limit - 5>
        public const int SpecTradingBuffBonus = 1; // Increases shop offer size and material selling ratio by 1% <Base - 5>
        public const int SpecMeltingBuffBonus = 5; // Increases % chance to get additional ingots when melting. <Base - 0%>
        public const int SpecQuestingBuffBonus = 5; // Reduces % time required to complete quests. <Base - 0%> <Limit - 50%>
        public const int SpecDungeonBuffBonus = 1; // Increases amount of time to defeat dungeon boss in seconds <Base - 30s>

        public ObservableDictionary<SpecializationType, int> SpecializationBuffs { get; set; }

        public ObservableDictionary<SpecializationType, int> SpecializationThresholds { get; set; }

        public ObservableDictionary<SpecializationType, int> SpecializationAmounts { get; set; }

        public string SpecCraftingText { get; set; }

        public Specialization()
        {
            SpecializationBuffs = new ObservableDictionary<SpecializationType, int>();
            SpecializationThresholds = new ObservableDictionary<SpecializationType, int>();
            SpecializationAmounts = new ObservableDictionary<SpecializationType, int>();

            CollectionInitializer.InitializeDictionary(SpecializationBuffs);
            CollectionInitializer.InitializeDictionary(SpecializationThresholds);
            CollectionInitializer.InitializeDictionary(SpecializationAmounts);

            UpdateSpecialization();
        }

        private static void SpecializationAmounts_Updated(object sender, EventArgs e)
        {
            User.Instance.CurrentHero?.Specialization.UpdateBuffs();
        }

        public void UpdateThresholds()
        {
            // Buff gains thresholds.
            SpecializationThresholds[SpecializationType.Blessing] = 10; // Amount increases every time a Blessing is bought.
            SpecializationThresholds[SpecializationType.Clicking] = 1000; // Amount increases every time user clicks on monster or boss.
            SpecializationThresholds[SpecializationType.Crafting] = 10; // Amount increases every time an artifact is crafted using recipe.
            SpecializationThresholds[SpecializationType.Trading] = 10; // Amount increases every time a Recipe is bought or Material is sold.
            SpecializationThresholds[SpecializationType.Melting] = 10; // Amount increases every time a material is melted.
            SpecializationThresholds[SpecializationType.Questing] = 10; // Amount increases every time a quest is completed.
            SpecializationThresholds[SpecializationType.Dungeon] = 10; // Amount increases every time a dungeon is finished.

            // Changes that depend on hero class.
            // Changing thresholds is easier to balance than changing buffconst.
            switch (User.Instance.CurrentHero?.HeroRace)
            {
                case HeroRace.Human:
                    SpecializationThresholds[SpecializationType.Crafting] = 5;
                    SpecializationThresholds[SpecializationType.Trading] = 5;

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

            SpecializationBuffs[SpecializationType.Blessing] = SpecializationAmounts[SpecializationType.Blessing] / SpecializationThresholds[SpecializationType.Blessing] * SpecBlessingBuffBonus;

            SpecializationBuffs[SpecializationType.Clicking] = SpecializationAmounts[SpecializationType.Clicking] / SpecializationThresholds[SpecializationType.Clicking] * SpecClickingBuffBonus;

            SpecializationBuffs[SpecializationType.Crafting] = Math.Min(SpecCraftingBuffBase + SpecializationAmounts[SpecializationType.Crafting] / SpecializationThresholds[SpecializationType.Crafting] * SpecCraftingBuffBonus, SpecCraftingBuffLimit);

            SpecializationBuffs[SpecializationType.Trading] = SpecTradingBuffBase + SpecializationAmounts[SpecializationType.Trading] / SpecializationThresholds[SpecializationType.Trading] * SpecTradingBuffBonus;

            SpecializationBuffs[SpecializationType.Melting] = SpecializationAmounts[SpecializationType.Melting] / SpecializationThresholds[SpecializationType.Melting] * SpecMeltingBuffBonus;

            SpecializationBuffs[SpecializationType.Questing] = Math.Min(SpecializationAmounts[SpecializationType.Questing] / SpecializationThresholds[SpecializationType.Questing] * SpecQuestingBuffBonus, SpecQuestingBuffLimit);

            SpecializationBuffs[SpecializationType.Dungeon] = SpecDungeonBuffBase + SpecializationAmounts[SpecializationType.Dungeon] / SpecializationThresholds[SpecializationType.Dungeon] * SpecDungeonBuffBonus;

            // Update crafting text (used in hero stats panel).
            SpecCraftingText = ((Rarity)SpecializationBuffs[SpecializationType.Crafting]).ToString();
        }

        public void UpdateSpecialization()
        {
            UpdateThresholds();
            UpdateBuffs();

            // Assign event handlers.
            // First remove the handler (if it's already been added) - otherwise it doesn't do anything.
            // This is to make sure the same handler isn't added twice.
            // https://stackoverflow.com/a/3598010/17360812
            SpecializationAmounts.SpecializationCollectionUpdated -= SpecializationAmounts_Updated;
            SpecializationAmounts.SpecializationCollectionUpdated += SpecializationAmounts_Updated;
        }
    }
}