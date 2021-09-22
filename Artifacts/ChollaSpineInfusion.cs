using ClickQuest.Enemies;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Has to be equipped with at least one other artifact. Causes all clicks to leave a spine in the target’s body.
	// Click damage dealt is increased by 2 (?) for each spine in the target.
	public class ChollaSpineInfusion : ArtifactFunctionality
	{
		private const int DamagePerSpine = 2;

		private Enemy _currentEnemy;
		private int _spineCount;
		
		public override void OnEquip()
		{
			if (User.Instance.CurrentHero.EquippedArtifacts.Count < 1)
			{
				// todo: Do not equip this artifact.
			}
		}

		public override void OnEnemyClick()
		{
			if (_currentEnemy != InterfaceController.CurrentEnemy)
			{
				_spineCount = 0;
				_currentEnemy = InterfaceController.CurrentEnemy;
			}
			else
			{
				CombatController.DealDamageToEnemy(DamagePerSpine * _spineCount, DamageType.Artifact);
				_spineCount++;
			}
		}

		public ChollaSpineInfusion()
		{
			Name = "Cholla Spine Infusion";
		}
	}
}