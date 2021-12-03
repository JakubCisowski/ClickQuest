using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Core.Items.Artifacts
{
    // Has to be equipped with at least one other artifact. Causes all clicks to leave a spine in the target’s body.
    // Click damage dealt is increased by 2 (?) for each spine in the target.
    public class ChollaSpineInfusion : ArtifactFunctionality
    {
        private const int DamagePerSpine = 2;

        private Enemy _currentEnemy;
        private int _spineCount;

        public override void OnEnemyClick(Enemy clickedEnemy)
        {
            if (_currentEnemy != clickedEnemy)
            {
                _spineCount = 0;
                _currentEnemy = clickedEnemy;
            }

            CombatController.DealDamageToEnemy(clickedEnemy, DamagePerSpine * _spineCount, DamageType.Artifact);
            _spineCount++;
        }

        public override void OnRegionLeave()
        {
            _spineCount = 0;
        }

        public ChollaSpineInfusion()
        {
            Name = "Cholla Spine Infusion";
        }
    }
}