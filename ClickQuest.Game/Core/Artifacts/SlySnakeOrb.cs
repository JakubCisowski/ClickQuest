using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Artifacts
{
	// Increases your Click Damage by 20, and your Aura Attack Speed by 10%.
	public class SlySnakeOrb : ArtifactFunctionality
	{
		private const int ClickDamageIncrease = 20;
		private const double AuraSpeedIncreaseMultiplier = 0.10;

		private double _auraSpeedIncreased;

		public override void OnEquip()
		{
			User.Instance.CurrentHero.ClickDamage += ClickDamageIncrease;

			_auraSpeedIncreased = User.Instance.CurrentHero.AuraAttackSpeed * AuraSpeedIncreaseMultiplier;

			User.Instance.CurrentHero.AuraAttackSpeed += _auraSpeedIncreased;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.ClickDamage -= ClickDamageIncrease;
			User.Instance.CurrentHero.AuraAttackSpeed -= _auraSpeedIncreased;

			_auraSpeedIncreased = 0;
		}

		public SlySnakeOrb()
		{
			Name = "Sly Snake Orb";
		}
	}
}