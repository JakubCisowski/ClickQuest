using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;

namespace ClickQuest.Artifacts
{
	// All non-artifact damage dealt by you is increased by 10% against Bosses.
	public class SilverGreatsword : ArtifactFunctionality
	{
		private const double DamageIncreasePercent = 0.10;
		
		public override void OnDealingDamage(int baseDamage)
		{
			if (InterfaceController.CurrentEnemy is Boss)
			{
				int damageDealt = (int) DamageIncreasePercent * baseDamage;
			
				CombatController.DealDamageToEnemy(damageDealt, DamageType.Artifact);
			}
		}

		public SilverGreatsword()
		{
			Name = "Silver Greatsword";
		}
	}
}