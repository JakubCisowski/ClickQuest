using System;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// After killing a monster, your next click is guaranteed to critically hit, and will deal 40% more damage.
public class GhostlyInfusion : ArtifactFunctionality
{
	private const double DamageModifier = 0.4;

	private bool _isNextClickEmpowered;

	public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
	{
		if (_isNextClickEmpowered)
		{
			_isNextClickEmpowered = false;

			if (clickDamageType == DamageType.Normal)
			{
				var criticalDamageDealt = (int)Math.Ceiling(clickDamage * User.Instance.CurrentHero.CritDamage);

				CombatHelper.DealDamageToCurrentEnemy(criticalDamageDealt, DamageType.Critical);
				CombatHelper.DealDamageToCurrentEnemy((int)(criticalDamageDealt * DamageModifier), DamageType.Magic);

				clickDamage = 0;
			}
			else if (clickDamageType == DamageType.Critical)
			{
				CombatHelper.DealDamageToCurrentEnemy((int)(clickDamage * DamageModifier), DamageType.Magic);
			}
		}
	}

	public override void OnKill()
	{
		_isNextClickEmpowered = true;
	}

	public GhostlyInfusion()
	{
		Name = "Ghostly Infusion";
	}
}