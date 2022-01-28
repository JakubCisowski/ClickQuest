using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities;

public class ArtifactTypeFunctionality
{
	public ArtifactType ArtifactType { get; set; }
	public string Description { get; set; }

	// Used when trying to equip an artifact to determine if it can be equipped.
	// Triggered: before equipping an Artifact, after checking the specific Artifact's CanBeEquipped.
	public virtual bool CanBeEquipped()
	{
		return true;
	}

	// Used upon equipping or re-equipping an artifact to grant some permanent bonuses.
	// Triggered: after equipping an Artifact, after granting Artifact-specific bonuses.
	public virtual void OnEquip()
	{
	}

	// Used upon unequipping an artifact to take back the bonuses granted in OnEquip.
	// Triggered: after unequipping an Artifact, after taking Artifact-specific bonuses.
	public virtual void OnUnequip()
	{
	}

	// Used to trigger on-click effects, such as a damage increase or a stacking bonus.
	// Triggered: after dealing all types of damage from a click to an enemy (after applying click and on-hit damage),
	// after triggering Artifact-specific bonuses.
	// clickedEnemy - the enemy that was clicked.
	public virtual void OnEnemyClick(Enemy clickedEnemy)
	{
	}

	// Used to trigger an effect based on enemy deaths.
	// Triggered: after killing an enemy and after granting victory bonuses, after triggering Artifact-specific bonuses,
	// but before spawning another monster
	// Also triggered when boss timer goes down to 0, regardless if the boss died or not.
	public virtual void OnKill()
	{
	}

	// Used to trigger an effect upon gaining experience.
	// Triggered: when gaining experience, after adding to the hero Experience property, after triggering Artifact-specific bonuses.
	// experienceGained - the amount of experience gained.
	public virtual void OnExperienceGained(ref int experienceGained)
	{
	}

	// Used to trigger an effect upon starting a quest.
	// Triggered: when starting a quest, after triggering Artifact-specific bonuses, before starting its timer.
	// quest - quest that is being started.
	public virtual void OnQuestStarted(Quest quest)
	{
	}
}