using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Requires three artifact slots to equip. Your click, poison and aura damage become zero.
	// Instead, each click you make on a Monster will give you loot and instantly defeat them. Does not work on Bosses.
	public class Transcendence : ArtifactFunctionality
	{
		private int _clickDamageDecreased;
		private int _poisonDamageDecreased;
		private double _auraDamageDecreased;

		public override void OnEquip()
		{
			// todo: levelupy nadal zwiększają dmg a nie powinny
			
			_clickDamageDecreased = User.Instance.CurrentHero.ClickDamage;
			_poisonDamageDecreased = User.Instance.CurrentHero.PoisonDamage;
			_auraDamageDecreased = User.Instance.CurrentHero.AuraDamage;

			User.Instance.CurrentHero.ClickDamage = 0;
			User.Instance.CurrentHero.PoisonDamage = 0;
			User.Instance.CurrentHero.AuraDamage = 0;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.ClickDamage = _clickDamageDecreased;
			User.Instance.CurrentHero.PoisonDamage = _poisonDamageDecreased;
			User.Instance.CurrentHero.AuraDamage = _auraDamageDecreased;
			
			_clickDamageDecreased = 0;
			_poisonDamageDecreased = 0;
			_auraDamageDecreased = 0;
		}

		public override void OnEnemyClick()
		{
			if (InterfaceController.CurrentEnemy is Monster monster)
			{
				CombatController.DealDamageToEnemy(monster.CurrentHealth, DamageType.Artifact);
			}
		}

		public Transcendence()
		{
			Name = "Transcendence";
			ArtifactSlotsRequired = 3;
		}
	}
}