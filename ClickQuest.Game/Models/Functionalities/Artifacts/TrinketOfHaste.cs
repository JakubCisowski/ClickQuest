using System;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// Quests take 25% less time to complete.
public class TrinketOfHaste : ArtifactFunctionality
{
	private const double QuestTimeReduced = 0.25;

	public override void OnQuestStarted(Quest quest)
	{
		quest.Duration -= (int)Math.Ceiling(quest.Duration * QuestTimeReduced);
		
		base.OnQuestStarted(quest);
	}

	public TrinketOfHaste()
	{
		Name = "Trinket of Haste";
	}
}