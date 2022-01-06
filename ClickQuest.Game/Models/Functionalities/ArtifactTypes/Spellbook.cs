using System.Linq;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models.Functionalities.ArtifactTypes;

public class Spellbook : ArtifactTypeFunctionality
{
	private const double AuraSpeedPerSpellbook = 0.05;
	private const double AuraBonusModifierWithThreeSpellbooks = 2;

	private bool _bonusStatsIncreased;

	public override void OnEquip()
	{
		var hasThreeSpellbooks = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Spellbook) == 3;

		if (hasThreeSpellbooks)
		{
			_bonusStatsIncreased = true;
			User.Instance.CurrentHero.AuraAttackSpeed += AuraSpeedPerSpellbook;

			User.Instance.CurrentHero.AuraAttackSpeed += 3 * (AuraBonusModifierWithThreeSpellbooks - 1) * AuraSpeedPerSpellbook;
		}
		else
		{
			User.Instance.CurrentHero.AuraAttackSpeed += AuraSpeedPerSpellbook;
		}
	}

	public override void OnUnequip()
	{
		if (_bonusStatsIncreased)
		{
			_bonusStatsIncreased = false;
			User.Instance.CurrentHero.AuraAttackSpeed -= AuraSpeedPerSpellbook;

			User.Instance.CurrentHero.AuraAttackSpeed -= 3 * (AuraBonusModifierWithThreeSpellbooks - 1) * AuraSpeedPerSpellbook;
		}
		else
		{
			User.Instance.CurrentHero.AuraAttackSpeed -= AuraSpeedPerSpellbook;
		}
	}

	public Spellbook()
	{
		ArtifactType = ArtifactType.Spellbook;
		Description = "Each equipped Spellbook increases your Aura Speed by 5% (additively). If you have three Spellbooks equipped, this bonus is doubled, to a total of 30%.";
	}
}