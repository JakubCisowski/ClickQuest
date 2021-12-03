using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.Artifacts
{
    // Gain 20% Critical Click Damage.
    public class MeatCleaver : ArtifactFunctionality
    {
        private const double CritDamageIncrease = 0.20;

        public override void OnEquip()
        {
            User.Instance.CurrentHero.CritDamage += CritDamageIncrease;
        }

        public override void OnUnequip()
        {
            User.Instance.CurrentHero.CritDamage -= CritDamageIncrease;
        }

        public MeatCleaver()
        {
            Name = "Meat Cleaver";
        }
    }
}