using System.Linq;
using System.Windows;
using ClickQuest.Controls;
using ClickQuest.Pages;
using ClickQuest.Player;

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

			bool isNoQuestActive = User.Instance.CurrentHero.Quests.All(x => x.EndDate == default);
			object currentFrameContent = (Application.Current.MainWindow as GameWindow)?.CurrentFrame.Content;
			bool isCurrentFrameContentARegion = currentFrameContent is RegionPage;

			if (isNoQuestActive && isCurrentFrameContentARegion)
			{
				var monsterButton = (currentFrameContent as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
				monsterButton.StartAuraTimer();
			}
		}

		public static MonsterButton GetCurrentMonsterButton()
		{
			object currentFrameContent = (Application.Current.MainWindow as GameWindow).CurrentFrame.Content;
			bool isCurrentFrameContentARegion = currentFrameContent is RegionPage;

			if (isCurrentFrameContentARegion)
			{
				return (currentFrameContent as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
			}

			return null;
		}
	}
}