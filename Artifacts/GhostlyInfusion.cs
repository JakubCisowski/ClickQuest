using ClickQuest.Extensions.CombatManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	public class GhostlyInfusion : ArtifactFunctionality
	{
		private const double DamageModifier = 1.1;
		
		private bool _isNextClickEmpowered = false;
		
		public override void OnEquip()
		{
			if (User.Instance.CurrentHero.EquippedArtifacts.Count < 0)
			{
				// Do not equip this artifact.
			}
		}

		public override void OnClick()
		{
			// todo: to nie działa jak intended
			if (_isNextClickEmpowered)
			{
				_isNextClickEmpowered = false;

				var monsterButton = CombatController.GetCurrentMonsterButton();
				monsterButton.DealDamageToMonster((int)(User.Instance.CurrentHero.ClickDamage * DamageModifier * User.Instance.CurrentHero.CritDamage), DamageType.Critical);
			}
		}

		public override void OnKill()
		{
			_isNextClickEmpowered = true;
		}

		public GhostlyInfusion()
		{
			Name = "Ghostly Infusion";
		}
	}
}