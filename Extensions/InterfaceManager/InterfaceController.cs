using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Controls;
using ClickQuest.Data.GameData;
using ClickQuest.Enemies;
using ClickQuest.Pages;
using Microsoft.CSharp.RuntimeBinder;

namespace ClickQuest.Extensions.InterfaceManager
{
	public static class InterfaceController
	{
		public static Enemy CurrentEnemy
		{
			get
			{
				object currentFrameContent = (Application.Current.MainWindow as GameWindow)?.CurrentFrame.Content;
				bool isCurrentFrameContentARegion = currentFrameContent is RegionPage;

				if (isCurrentFrameContentARegion)
				{
					return (currentFrameContent as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault()?.Monster;
				}

				bool isCurrentFrameContentADungeon = currentFrameContent is DungeonBossPage;

				if (isCurrentFrameContentADungeon)
				{
					return (currentFrameContent as DungeonBossPage).Boss;
				}

				return null;
			}
		}

		public static MonsterButton CurrentMonsterButton
		{
			get
			{
				bool isCurrentFrameContentARegion = GameData.CurrentPage is RegionPage;

				if (isCurrentFrameContentARegion)
				{
					return (GameData.CurrentPage as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
				}

				return null;
			}
		}

		public static DungeonBossPage CurrentBossPage
		{
			get
			{
				bool isCurrentFrameContentARegion = GameData.CurrentPage is DungeonBossPage;

				if (isCurrentFrameContentARegion)
				{
					return GameData.CurrentPage as DungeonBossPage;
				}

				return null;
			}
		}

		public static void RefreshStatsAndEquipmentPanelsOnCurrentPage()
		{
			try
			{
				dynamic p = GameData.CurrentPage;
				p.StatsFrame.Refresh();
				p.EquipmentFrame.Refresh();
			}
			catch (RuntimeBinderException)
			{
				// No stats frame on this page!
				// Best solution according to:
				// https://stackoverflow.com/a/5768449/14770235
			}
		}

		public static void ChangePage(Page destinationPage, string locationInfoText)
		{
			var window = Application.Current.MainWindow as GameWindow;
			window.CurrentFrame.Navigate(destinationPage);
			window.LocationInfo = locationInfoText;

			GameData.CurrentPage = destinationPage;
			RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}
	}
}