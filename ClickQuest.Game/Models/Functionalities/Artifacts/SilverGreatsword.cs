using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// All non-magic damage dealt by you is increased by 10% against Bosses.
public class SilverGreatsword : ArtifactFunctionality
{
	private const double DamageIncreasePercent = 0.10;

	public override void OnDealingDamage(ref int baseDamage)
	{
		if (InterfaceHelper.CurrentEnemy is Boss)
		{
			var bonusDamage = (int)(DamageIncreasePercent * baseDamage);

			baseDamage += bonusDamage;
		}
	}

	public SilverGreatsword()
	{
		Name = "Silver Greatsword";
	}
}