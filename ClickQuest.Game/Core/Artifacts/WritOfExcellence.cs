using ClickQuest.Game.Core.Heroes.Buffs;
using ClickQuest.Game.Core.Items;

namespace ClickQuest.Game.Core.Artifacts
{
	// Blessings on you have a doubled effectiveness.
	public class WritOfExcellence : ArtifactFunctionality
	{
		private const double EffectivenessIncrease = 2.0;

		public override void OnBlessingStarted(Blessing blessing)
		{
			blessing.Buff = (int) (blessing.Buff * EffectivenessIncrease);
		}

		public WritOfExcellence()
		{
			Name = "Writ of Excellence";
		}
	}
}