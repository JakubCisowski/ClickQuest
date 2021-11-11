using System.Linq;
using System.Windows;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.Core.Artifacts
{
	// Has to be equipped with at least one other artifact. 
	// After killing a monster, your next click is guaranteed to critically hit, and will deal 10% more damage.
	public class GhostlyInfusion : ArtifactFunctionality
	{
		private const double DamageModifier = 0.1;

		private bool _isNextClickEmpowered;

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
		
		public override bool CanBeUnequipped()
		{
			bool isFighting = GameAssets.CurrentPage is RegionPage or DungeonBossPage;

			if (isFighting)
			{
				AlertBox.Show("You cannot unequip artifacts while in combat.", MessageBoxButton.OK);
				return false;
			}

			bool isQuesting = User.Instance.CurrentHero.Quests.Any(x => x.EndDate != default);

			if (isQuesting)
			{
				AlertBox.Show("You cannot unequip artifacts while questing.", MessageBoxButton.OK);
				return false;
			}
			
			return true;
		}

		public override void OnDealingClickDamage(ref int clickDamage, DamageType clickDamageType)
		{
			if (_isNextClickEmpowered)
			{
				_isNextClickEmpowered = false;

				if (clickDamageType == DamageType.Normal)
				{
					int criticalDamageDealt = (int) (clickDamage * User.Instance.CurrentHero.CritDamage);

					CombatController.DealDamageToCurrentEnemy(criticalDamageDealt, DamageType.Critical);
					CombatController.DealDamageToCurrentEnemy((int) (criticalDamageDealt * DamageModifier), DamageType.Artifact);
					
					clickDamage = 0;
				}
				else if (clickDamageType == DamageType.Critical)
				{
					CombatController.DealDamageToCurrentEnemy((int) (clickDamage * DamageModifier), DamageType.Artifact);
				}
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