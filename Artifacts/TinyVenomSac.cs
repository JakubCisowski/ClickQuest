using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Increases your Poison Damage by 1.
	public class TinyVenomSac : ArtifactFunctionality
	{
		private const int PoisonDamageIncrease = 1;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.PoisonDamage += PoisonDamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.PoisonDamage -= PoisonDamageIncrease;
		}

		public TinyVenomSac()
		{
			Name = "Tiny Venom Sac";
		}
	}
}