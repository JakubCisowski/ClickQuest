using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts;

// Increases Aura Attack Speed by 50% (additively).
public class RakshasasNecklace : ArtifactFunctionality
{
	private const double AuraAttackSpeedBonus = 0.5;

	public override void OnEquip()
	{
		User.Instance.CurrentHero.AuraAttackSpeed += AuraAttackSpeedBonus;
	}

	public override void OnUnequip()
	{
		User.Instance.CurrentHero.AuraAttackSpeed -= AuraAttackSpeedBonus;
	}

	public RakshasasNecklace()
	{
		Name = "Rakshasa's Necklace";
	}
}