using ClickQuest.Data;
using ClickQuest.Pages;

namespace ClickQuest.Extensions.QuestManager
{
	public static class QuestController
	{
		public static void RerollQuests()
		{
			(GameData.Pages["QuestMenu"] as QuestMenuPage).RerollQuests();
		}
	}
}