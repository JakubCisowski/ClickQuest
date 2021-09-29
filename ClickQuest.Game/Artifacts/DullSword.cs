using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Increases your Click Damage by 3.
	public class DullSword : ArtifactFunctionality
	{
		private const int DamageIncrease = 3;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.ClickDamage += DamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.ClickDamage -= DamageIncrease;
		}

		public DullSword()
		{
			Name = "Dull Sword";
		}
	}
}