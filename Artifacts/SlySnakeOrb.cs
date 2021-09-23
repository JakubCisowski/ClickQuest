using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Increases your Click Damage by 20, and your Aura Attack Speed by 10%.
	public class SlySnakeOrb : ArtifactFunctionality
	{
		private const int ClickDamageIncrease = 20;
		private const double AuraDamageIncreaseMultiplier = 0.10;// todo: zamienić to na aura attack speed

		public override void OnEquip()
		{
			User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
		}

		public SlySnakeOrb()
		{
			Name = "Sly Snake Orb";
		}
	}
}