using System.Linq;
using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.Models.Functionalities;

public class ArtifactFunctionality
{
	public string Name { get; set; }
	public int ArtifactSlotsRequired { get; set; } = 1;
	public ArtifactTypeFunctionality ArtifactTypeFunctionality { get; set; }

	// Used when trying to equip an artifact to determine if it can be equipped.
	// Triggered: before equipping an Artifact.
	public virtual bool CanBeEquipped()
	{
		var isFighting = GameAssets.CurrentPage is RegionPage or DungeonBossPage;

		if (isFighting)
		{
			AlertBox.Show("You cannot equip artifacts while in combat.", MessageBoxButton.OK);
			return false;
		}

		var isQuesting = User.Instance.CurrentHero.Quests.Any(x => x.EndDate != default);

		if (isQuesting)
		{
			AlertBox.Show("You cannot equip artifacts while questing.", MessageBoxButton.OK);
			return false;
		}

		var equippedArtifactsSlots = User.Instance.CurrentHero.EquippedArtifacts.Sum(x => x.ArtifactFunctionality.ArtifactSlotsRequired);

		var slotText = ArtifactSlotsRequired == 1 ? "slot" : "slots";

		if (3 - equippedArtifactsSlots < ArtifactSlotsRequired)
		{
			AlertBox.Show($"This artifact cannot be equipped right now - it requires {ArtifactSlotsRequired} free {slotText} to use.", MessageBoxButton.OK);
			return false;
		}

		if (!ArtifactTypeFunctionality.CanBeEquipped())
		{
			return false;
		}

		return true;
	}

	// Used when trying to unequip an artifact to determine if it can be unequipped.
	// Triggered: before unequipping an Artifact.
	public virtual bool CanBeUnequipped()
	{
		var isFighting = GameAssets.CurrentPage is RegionPage or DungeonBossPage;

		if (isFighting)
		{
			AlertBox.Show("You cannot unequip artifacts while in combat.", MessageBoxButton.OK);
			return false;
		}

		var isQuesting = User.Instance.CurrentHero.Quests.Any(x => x.EndDate != default);

		if (isQuesting)
		{
			AlertBox.Show("You cannot unequip artifacts while questing.", MessageBoxButton.OK);
			return false;
		}

		var isInfusionEquipped = User.Instance.CurrentHero.EquippedArtifacts.Any(x => x.ArtifactType == ArtifactType.Infusion);
		var isOnlyOtherArtifact = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType != ArtifactType.Infusion) == 1;

		if (isInfusionEquipped && isOnlyOtherArtifact && ArtifactTypeFunctionality.ArtifactType != ArtifactType.Infusion)
		{
			AlertBox.Show("You cannot unequip this artifact right now - you have an artifact equipped that requires at least one other artifact.", MessageBoxButton.OK);
			return false;
		}

		return true;
	}

	// Used upon equipping or re-equipping an artifact to grant some permanent bonuses.
	// Triggered: after equipping an Artifact, and after loading data from User.json.
	public virtual void OnEquip()
	{
	}

	// Used upon unequipping an artifact to take back the bonuses granted in OnEquip.
	// Triggered: after unequipping an Artifact, and before exiting the game and saving user data (to prevent bonuses from persisting forever).
	public virtual void OnUnequip()
	{
	}

	// Used to trigger on-click effects, such as a damage increase or a stacking bonus.
	// Triggered: after dealing all types of damage from a click to an enemy (after applying click and on-hit damage).
	// clickedEnemy - the enemy that was clicked.
	public virtual void OnEnemyClick(Enemy clickedEnemy)
	{
	}

	// Used to modify or trigger an effect based on the amount of any damage dealt (click, on-hit, poison, aura) except artifact damage.
	// Triggered: before dealing any damage to an enemy, and before triggering other OnDealingXDamage effects.
	// damage - the amount of damage dealt.
	public virtual void OnDealingDamage(ref int damage)
	{
	}

	// Used to modify or trigger an effect based on the amount of click damage dealt (regardless if critical or not).
	// Triggered: before dealing click damage to an enemy, but after triggering the OnDealingDamage effect.
	// clickDamage - the amount of damage dealt; clickDamageType - determines if the damage was critical or not.
	public virtual void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
	}

	// Used to modify or trigger an effect based on the amount of poison dealt.
	// Triggered: before dealing poison damage to an enemy, but after triggering the OnDealingDamage effect.
	// poisonDamage - the amount of damage dealt.
	public virtual void OnDealingPoisonDamage(ref int poisonDamage)
	{
	}

	// Used to modify or trigger an effect based on the amount of aura damage dealt.
	// Triggered: before dealing aura damage to an enemy, but after triggering the OnDealingDamage effect.
	// auraDamage - the amount of damage dealt.
	public virtual void OnDealingAuraDamage(ref int auraDamage)
	{
	}

	// Used to modify or trigger an effect based on the amount of artifact damage dealt.
	// Triggered: before dealing artifact damage to an enemy, but after triggering the OnDealingDamage effect.
	// artifactDamage - the amount of damage dealt.
	public virtual void OnDealingArtifactDamage(ref int artifactDamage)
	{
	}

	// Used to trigger an effect based on enemy deaths.
	// Triggered: after killing an enemy and after granting victory bonuses, but before spawning another monster
	// Also triggered when boss timer goes down to 0, regardless if the boss died or not.
	public virtual void OnKill()
	{
	}

	// Used to trigger an effect when entering a region.
	// Triggered: when switching page to a region, after the ChangePage method is invoked.
	public virtual void OnRegionEnter()
	{
	}

	// Used to trigger an effect when leaving a region.
	// Triggered: when switching page from a region, and before exiting the game and saving user data (to prevent bonuses from persisting forever).
	public virtual void OnRegionLeave()
	{
	}

	// Used to trigger an effect upon gaining experience.
	// Triggered: when gaining experience, after adding to the hero Experience property.
	// experienceGained - the amount of experience gained.
	public virtual void OnExperienceGained(int experienceGained)
	{
	}

	// Used to trigger an effect upon gaining a blessing.
	// Triggered: when gaining a blessing, before increasing a hero's stats and starting the blessing timer.
	// blessing - blessing that is being gained.
	public virtual void OnBlessingStarted(Blessing blessing)
	{
	}

	// Used to trigger an effect upon starting a quest.
	// Triggered: when starting a quest, before starting its timer.
	// quest - quest that is being started.
	public virtual void OnQuestStarted(Quest quest)
	{
	}
}