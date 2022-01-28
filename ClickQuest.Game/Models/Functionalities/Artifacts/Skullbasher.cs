using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Every other click you make deals an additional 50 damage.
public class Skullbasher : ArtifactFunctionality
{
	private const int DamageDealt = 50;

	private bool _isNextClickEmpowered;

	public override void OnEnemyClick(Enemy clickedEnemy)
	{
		if (!_isNextClickEmpowered)
		{
			_isNextClickEmpowered = true;
			return;
		}

		_isNextClickEmpowered = false;

		CombatHelper.DealDamageToEnemy(clickedEnemy, DamageDealt, DamageType.Magic);
	}

	public override void OnRegionLeave()
	{
		_isNextClickEmpowered = false;
	}

	public Skullbasher()
	{
		Name = "Skullbasher";
	}
}