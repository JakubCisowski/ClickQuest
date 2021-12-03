using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.Extensions.Quests
{
	public static class QuestController
	{
		public static void RerollQuests()
		{
			(GameAssets.Pages["QuestMenu"] as QuestMenuPage).RerollQuests();
		}
	}
}