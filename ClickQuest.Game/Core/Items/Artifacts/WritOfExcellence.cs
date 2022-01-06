using ClickQuest.Game.Core.Heroes.Buffs;

namespace ClickQuest.Game.Core.Items.Artifacts;

// Blessings on you have a doubled effectiveness.
public class WritOfExcellence : ArtifactFunctionality
{
	private const double EffectivenessIncrease = 2.0;

	public override void OnBlessingStarted(Blessing blessing)
	{
		blessing.Buff = (int)(blessing.Buff * EffectivenessIncrease);
	}

	public WritOfExcellence()
	{
		Name = "Writ of Excellence";
	}
}