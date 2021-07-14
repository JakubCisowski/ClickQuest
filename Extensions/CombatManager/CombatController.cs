using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Pages;
using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;
using System.Linq;
using System.Windows;

namespace ClickQuest.Extensions.CombatManager
{
	public static class CombatController
	{
		public static void StartAuraTimerOnEachRegion()
		{
			if ((Application.Current.MainWindow as GameWindow).CurrentFrame.Content is RegionPage regionPage)
			{
				var monsterButton = regionPage.RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
				monsterButton.StartAuraTimer();

				// foreach (var ctrl in regionPage.RegionPanel.Children)
				// {
				// 	if (ctrl is MonsterButton monsterButton)
				// 	{
				// 		monsterButton.StartAuraTimer();
				// 		break;
				// 	}
				// }
			}
		}
	}
}