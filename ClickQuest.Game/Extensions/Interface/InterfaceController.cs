using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Core.Enemies;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Pages;
using ClickQuest.Game.UserInterface.Windows;
using Microsoft.CSharp.RuntimeBinder;

namespace ClickQuest.Game.Extensions.Interface
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
				bool isCurrentFrameContentARegion = GameAssets.CurrentPage is RegionPage;

				if (isCurrentFrameContentARegion)
				{
					return (GameAssets.CurrentPage as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
				}

				return null;
			}
		}

		public static DungeonBossPage CurrentBossPage
		{
			get
			{
				bool isCurrentFrameContentARegion = GameAssets.CurrentPage is DungeonBossPage;

				if (isCurrentFrameContentARegion)
				{
					return GameAssets.CurrentPage as DungeonBossPage;
				}

				return null;
			}
		}

		public static void RefreshStatsAndEquipmentPanelsOnCurrentPage()
		{
			try
			{
				dynamic p = GameAssets.CurrentPage;
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
			try
			{
				dynamic p = GameAssets.CurrentPage;
				(p.EquipmentFrame.Content as EquipmentPage).SaveScrollbarOffset();
			}
			catch (RuntimeBinderException)
			{
			}

			var window = Application.Current.MainWindow as GameWindow;
			window.CurrentFrame.Navigate(destinationPage);
			window.LocationInfo = locationInfoText;

			GameAssets.CurrentPage = destinationPage;
			RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}
	}
}