using ClickQuest.Game.Core.Player;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;

namespace ClickQuest.Game.Core.Items.Artifacts
{
    // Your poison damage can now critically strike. The probability is calculated using your Critical Click Chance, and the damage using your Critical Click Damage.
    public class WillOfTheAncients : ArtifactFunctionality
    {
        public override void OnDealingPoisonDamage(ref int poisonDamage)
        {
            double critChance = User.Instance.CurrentHero.CritChance;
            double critDamage = User.Instance.CurrentHero.CritDamage;
            double randomizedValue = Rng.Next(1, 101) / 100d;

            if (randomizedValue <= critChance)
            {
                poisonDamage = (int)(poisonDamage * critDamage);
            }
        }

        public WillOfTheAncients()
        {
            Name = "Will of the Ancients";
        }
    }
}