using ClickQuest.Game.Heroes.Buffs;
using ClickQuest.Game.Items;

namespace ClickQuest.Game.Artifacts
{
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