using ClickQuest.Heroes.Buffs;
using ClickQuest.Items;

namespace ClickQuest.Artifacts
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