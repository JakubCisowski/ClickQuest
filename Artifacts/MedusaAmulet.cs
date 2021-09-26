using ClickQuest.Data.GameData;
using ClickQuest.Items;
using ClickQuest.Pages;
using ClickQuest.Places;
using ClickQuest.Player;

namespace ClickQuest.Artifacts
{
	// While on a Region, each monster you kill increases your Click Damage by 2, Critical Click Chance by 1% and Critical Click Damage by 1%.
	// Stacks up to 20 times. Leaving the region removes all stacks.
	public class MedusaAmulet : ArtifactFunctionality
	{
		private const int ClickDamagePerStack = 2;
		private const double CritChancePerStack = 0.01;
		private const double CritDamagePerStack = 0.01;
		private const int MaxStacks = 20;

		private Region _currentRegion;
		private int _stackCount;

		public override void OnRegionEnter()
		{
			_currentRegion = (GameData.CurrentPage as RegionPage)?.Region;
		}

		public override void OnRegionLeave()
		{
			_currentRegion = default;

			User.Instance.CurrentHero.ClickDamage -= _stackCount * ClickDamagePerStack;
			User.Instance.CurrentHero.CritChance -= _stackCount * CritChancePerStack;
			User.Instance.CurrentHero.CritDamage -= _stackCount * CritDamagePerStack;

			_stackCount = 0;
		}

		public override void OnKill()
		{
			if (_stackCount < MaxStacks)
			{
				_stackCount++;

				User.Instance.CurrentHero.ClickDamage += ClickDamagePerStack;
				User.Instance.CurrentHero.CritChance += CritChancePerStack;
				User.Instance.CurrentHero.CritDamage += CritDamagePerStack;
			}
		}

		public MedusaAmulet()
		{
			Name = "Medusa Amulet";
		}
	}
}