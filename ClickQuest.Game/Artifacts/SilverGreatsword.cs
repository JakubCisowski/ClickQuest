using ClickQuest.Game.Enemies;
using ClickQuest.Game.Extensions.InterfaceManager;
using ClickQuest.Game.Items;

namespace ClickQuest.Game.Artifacts
{
	// All non-artifact damage dealt by you is increased by 10% against Bosses.
	public class SilverGreatsword : ArtifactFunctionality
	{
		private const double DamageIncreasePercent = 0.10;

		public override void OnDealingDamage(ref int baseDamage)
		{
			if (InterfaceController.CurrentEnemy is Boss)
			{
				int bonusDamage = (int) (DamageIncreasePercent * baseDamage);

				baseDamage += bonusDamage;
			}
		}

		public SilverGreatsword()
		{
			Name = "Silver Greatsword";
		}
	}
}