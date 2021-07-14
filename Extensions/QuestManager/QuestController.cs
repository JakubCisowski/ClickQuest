using ClickQuest.Data;
using ClickQuest.Pages;
using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;

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