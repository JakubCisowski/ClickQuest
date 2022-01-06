using System;
using System.Linq;
using ClickQuest.Game.Core.Adventures;
using ClickQuest.Game.Core.Items.Types;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Items.ArtifactTypes;

public class Idle : ArtifactTypeFunctionality
{
	private const int ThreePieceQuantityIncrease = 1;
	private const double TwoPieceXPModifier = 0.20;
	private const double OnePieceDurationModifier = 0.10;

	public override void OnQuestStarted(Quest quest)
	{
		var amountOfIdleArtifacts = User.Instance.CurrentHero.EquippedArtifacts.Count(x => x.ArtifactType == ArtifactType.Idle);

		if (amountOfIdleArtifacts == 3)
		{
			foreach (var pattern in quest.QuestRewardPatterns)
			{
				pattern.Quantity += ThreePieceQuantityIncrease;
			}
		}

		if (amountOfIdleArtifacts >= 2)
		{
			// TODO
		}

		if (amountOfIdleArtifacts >= 1)
		{
			quest.Duration -= (int)Math.Ceiling(quest.Duration * OnePieceDurationModifier);
		}
	}

	public Idle()
	{
		ArtifactType = ArtifactType.Idle;
		Description = "Gain bonuses based on the amount of Idle Artifacts equipped: (1) Quest durations reduced by 10%; (2) Also, Quest XP increased by 20%; (3) Also, the Quantity of all items received from Quests increased by 1.";
	}
}