﻿using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Causes all clicks to leave a spine in the target’s body.
// Click damage dealt is increased by 6 for each spine in the target.
public class ChollaSpineInfusion : ArtifactFunctionality
{
	private const int DamagePerSpine = 6;

	private Enemy _currentEnemy;
	private int _spineCount;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		if (_currentEnemy != clickedEnemy)
		{
			_spineCount = 0;
			_currentEnemy = clickedEnemy;
		}

		CombatHelper.DealDamageToEnemy(clickedEnemy, DamagePerSpine * _spineCount, DamageType.Magic);
		_spineCount++;
		
		base.OnEnemyClick(clickedEnemy);
	}

	public override void OnRegionLeave()
	{
		_spineCount = 0;
	}

	public ChollaSpineInfusion()
	{
		Name = "Cholla Spine Infusion";
	}
}