using ClickQuest.Controls;
using ClickQuest.Data;
using ClickQuest.Pages;
using ClickQuest.Player;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Dynamic;
using System.Linq;
using System.Windows;

namespace ClickQuest.Extensions.CombatManager
{
	public static class CombatController
	{
		public static void StartAuraTimerOnCurrentRegion()
		{
			bool isCurrentHeroSelected = User.Instance.CurrentHero != null;
			
			if (!isCurrentHeroSelected)
			{
				return;
			}
			
			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default(DateTime));
			var currentFrameContent = (Application.Current.MainWindow as GameWindow).CurrentFrame.Content;
			bool isCurrentFrameContentARegion = currentFrameContent is RegionPage;

			if (isNoQuestActive && isCurrentFrameContentARegion)
			{
				var monsterButton = (currentFrameContent as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
				monsterButton.StartAuraTimer();
			}
		}
	}
}