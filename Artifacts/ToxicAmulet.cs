using ClickQuest.Items;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// Your poison damage is increased by 20%.
	public class ToxicAmulet : ArtifactFunctionality
	{
		private const double PoisonDamageModifier = 1.2;

		private int _amountIncreased;

		public override void OnEquip()
		{
			// todo: jakoś to refreshować, żeby nie trzeba było re-equipować artefaktu
			_amountIncreased = (int) (User.Instance.CurrentHero.PoisonDamage * PoisonDamageModifier);
			User.Instance.CurrentHero.PoisonDamage += _amountIncreased;
		}

		public override void OnUnequip()
		{
			User.Instance.CurrentHero.PoisonDamage -= _amountIncreased;
		}

		public ToxicAmulet()
		{
			Name = "Toxic Amulet";
		}
	}
}