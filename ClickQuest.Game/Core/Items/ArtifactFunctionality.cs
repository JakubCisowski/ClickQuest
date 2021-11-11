using System.Linq;
using System.Windows;
using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.Core.Items
{
	public class ArtifactFunctionality
	{
		public string Name { get; set; }
		public int ArtifactSlotsRequired { get; set; } = 1;

		// Use when trying to equip an artifact to determine if it can be equipped.
		public virtual bool CanBeEquipped()
		{
			bool isFighting = GameAssets.CurrentPage is RegionPage or DungeonBossPage;

			if (isFighting)
			{
				AlertBox.Show("You cannot equip artifacts while in combat.", MessageBoxButton.OK);
				return false;
			}

			bool isQuesting = User.Instance.CurrentHero.Quests.Any(x => x.EndDate != default);

			if (isQuesting)
			{
				AlertBox.Show("You cannot equip artifacts while questing.", MessageBoxButton.OK);
				return false;
			}

			int equippedArtifactsSlots = User.Instance.CurrentHero.EquippedArtifacts.Sum(x => x.ArtifactFunctionality.ArtifactSlotsRequired);

			string slotText = ArtifactSlotsRequired == 1 ? "slot" : "slots";

			if (3 - equippedArtifactsSlots < ArtifactSlotsRequired)
			{
				AlertBox.Show($"This artifact cannot be equipped right now - it requires {ArtifactSlotsRequired} free {slotText} to use.", MessageBoxButton.OK);
				return false;
			}

			return true;
		}

		// Use when trying to unequip an artifact to determine if it can be unequipped.
		public virtual bool CanBeUnequipped()
		{
			bool isFighting = GameAssets.CurrentPage is RegionPage or DungeonBossPage;

			if (isFighting)
			{
				AlertBox.Show("You cannot unequip artifacts while in combat.", MessageBoxButton.OK);
				return false;
			}

			bool isQuesting = User.Instance.CurrentHero.Quests.Any(x => x.EndDate != default);

			if (isQuesting)
			{
				AlertBox.Show("You cannot unequip artifacts while questing.", MessageBoxButton.OK);
				return false;
			}
			
			// This should be ignored in infusion-type artifacts to allow them to be unequipped freely.
			
			bool isInfusionEquipped = User.Instance.CurrentHero.EquippedArtifacts.Any(x => x.ArtifactType == ArtifactType.Infusion);
			bool isOnlyOtherArtifact = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType != ArtifactType.Infusion) == 1;

			if (isInfusionEquipped && isOnlyOtherArtifact)
			{
				AlertBox.Show("You cannot unequip this artifact right now - you have an artifact equipped that requires at least one other artifact.", MessageBoxButton.OK);
				return false;
			}

			return true;
		}

		// Use when increasing base stats.
		public virtual void OnEquip()
		{
		}

		// Use to decrease base stats that have previously been increased.
		public virtual void OnUnequip()
		{
		}

		// Use to deal bonus damage upon clicking.
		public virtual void OnEnemyClick(Enemy clickedEnemy)
		{
		}

		// Use to increase ALL damage dealt (eg. by a percentage).
		public virtual void OnDealingDamage(ref int damage)
		{
		}

		// Use to increase click damage dealt (eg. by a percentage).
		public virtual void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
		{
		}

		// Use to increase poison damage dealt (eg. by a percentage).
		public virtual void OnDealingPoisonDamage(ref int poisonDamage)
		{
		}

		public virtual void OnDealingAuraDamage(ref int auraDamage)
		{
		}

		// Use to trigger on-kill effects.
		public virtual void OnKill()
		{
		}

		// Use to trigger region-based utility effects (eg. increased drop rate).
		public virtual void OnRegionEnter()
		{
		}

		// Use to revert the above utility effects.
		public virtual void OnRegionLeave()
		{
		}

		// Use for effects that trigger on experience gained (eg. bonus experience).
		public virtual void OnExperienceGained(int experienceGained)
		{
		}

		// Use for effects that increase blessing effectiveness.
		public virtual void OnBlessingStarted(Blessing blessing)
		{
		}

		// Use for effects that empower quests (eg. decrease duration).
		public virtual void OnQuestStarted(Quest quest)
		{
		}
	}
}