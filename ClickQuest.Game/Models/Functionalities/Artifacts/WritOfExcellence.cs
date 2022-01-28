using System;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Blessings on you have a doubled effectiveness.
public class WritOfExcellence : ArtifactFunctionality
{
	private const double EffectivenessIncrease = 2.0;

	public override void OnBlessingStarted(Blessing blessing)
	{
		blessing.Buff = (int)Math.Ceiling(blessing.Buff * EffectivenessIncrease);
	}

	public WritOfExcellence()
	{
		Name = "Writ of Excellence";
	}
}