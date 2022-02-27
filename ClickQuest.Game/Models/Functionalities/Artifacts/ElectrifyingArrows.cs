using System;
using System.Linq;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

public class ElectrifyingArrows : ArtifactFunctionality, IConsumable
{
	private const double PercentageHealthDamageAgainstMonsters = 0.20;
	private const double PercentageHealthDamageAgainstBosses = 0.005;
	
	public override void OnDealingMagicDamage(ref int magicDamage)
	{
		if (magicDamage <= 0)
		{
			return;
		}
		
		var rangedWeaponArtifact = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.ArtifactType == ArtifactType.RangedWeapon);

		if (rangedWeaponArtifact is null)
		{
			return;
		}

		var ammunitionArtifact = User.Instance.CurrentHero.EquippedArtifacts.FirstOrDefault(x => x.ArtifactFunctionality == this);

		if (ammunitionArtifact is null)
		{
			return;
		}

		Consume(ammunitionArtifact);
	}

	public void Consume(Artifact ammunitionArtifact)
	{
		ammunitionArtifact.RemoveItem(1);

		var currentEnemy = InterfaceHelper.CurrentEnemy;
		int damageDealt;
		
		if (currentEnemy is Monster)
		{
			damageDealt = (int)Math.Ceiling(currentEnemy.CurrentHealth * PercentageHealthDamageAgainstMonsters);
		}
		else
		{
			damageDealt = (int)Math.Ceiling(currentEnemy.CurrentHealth * PercentageHealthDamageAgainstBosses);
		}
		
		CombatHelper.DealDamageToCurrentEnemy(damageDealt, DamageType.OnHit);

		foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
		{
			artifact.ArtifactFunctionality.OnConsumed(ammunitionArtifact);
		}
	}

	public ElectrifyingArrows()
	{
		Name = "Electrifying Arrows";
		ArtifactSlotsRequired = 0;
	}
}