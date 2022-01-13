﻿using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities;

public class AffixFunctionality
{
	public Affix Affix { get; set; }

	// Use to increase poison damage dealt (eg. by a percentage).
	public virtual void OnDealingPoisonDamage(ref int poisonDamage)
	{
	}
}