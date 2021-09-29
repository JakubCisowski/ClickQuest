using ClickQuest.Game.Data.GameData;
using ClickQuest.Game.Pages;

namespace ClickQuest.Game.Extensions.QuestManager
{
	public static class QuestController
	{
		public static void RerollQuests()
		{
			(GameData.Pages["QuestMenu"] as QuestMenuPage).RerollQuests();
		}
	}
}