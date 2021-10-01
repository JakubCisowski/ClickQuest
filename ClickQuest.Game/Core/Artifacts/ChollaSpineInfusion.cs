using System.Windows;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.Extensions.UserInterface;
using ClickQuest.Game.UserInterface.Controls;

namespace ClickQuest.Game.Core.Artifacts
{
	// Has to be equipped with at least one other artifact. Causes all clicks to leave a spine in the target’s body.
	// Click damage dealt is increased by 2 (?) for each spine in the target.
	public class ChollaSpineInfusion : ArtifactFunctionality
	{
		private const int DamagePerSpine = 2;

		private Enemy _currentEnemy;
		private int _spineCount;

		public override bool CanBeEquipped()
		{
			bool canBeEquippedBase = base.CanBeEquipped();

			if (!canBeEquippedBase)
			{
				return false;
			}

			bool isAnArtifactEquipped = User.Instance.CurrentHero.EquippedArtifacts.Count > 0;

			if (isAnArtifactEquipped)
			{
				return true;
			}

			AlertBox.Show("This artifact cannot be equipped right now - it requires at least 1 other artifact to be equipped.", MessageBoxButton.OK);
			return false;
		}

		public override void OnEnemyClick()
		{
			if (_currentEnemy != InterfaceController.CurrentEnemy)
			{
				_spineCount = 0;
				_currentEnemy = InterfaceController.CurrentEnemy;
			}

			CombatController.DealDamageToEnemy(DamagePerSpine * _spineCount, DamageType.Artifact);
			_spineCount++;
		}

		public ChollaSpineInfusion()
		{
			Name = "Cholla Spine Infusion";
		}
	}
}