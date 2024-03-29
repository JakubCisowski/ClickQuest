﻿using System.Linq;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Can be consumed by critically clicking. Deals 250 damage.
public class SharpArrows : ArtifactFunctionality, IConsumable
{
	private const int DamageDealt = 250;
	
	public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
		if (clickDamageType != DamageType.Critical)
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
		CombatHelper.DealDamageToCurrentEnemy(DamageDealt);
		
		foreach (var artifact in User.Instance.CurrentHero.EquippedArtifacts)
		{
			artifact.ArtifactFunctionality.OnConsumed(ammunitionArtifact);
		}
	}

	public SharpArrows()
	{
		Name = "Sharp Arrows";
		ArtifactSlotsRequired = 0;
	}
}