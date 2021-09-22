using System.Linq;
using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Interfaces;
using ClickQuest.Items;
using ClickQuest.Pages;

namespace ClickQuest.Artifacts
{
	// Every other click you make deals an additional 10 (?) damage.
	public class Skullbasher : ArtifactFunctionality
	{
		private const int DamageDealt = 10;
		
		private bool _isNextClickEmpowered;

		public override void OnEnemyClick()
		{
			if (!_isNextClickEmpowered)
			{
				_isNextClickEmpowered = true;
				return;
			}

			_isNextClickEmpowered = false;

			CombatController.DealDamageToEnemy(DamageDealt, DamageType.Artifact);
		}
		
		public Skullbasher()
		{
			Name = "Skullbasher";
		}
	}
}