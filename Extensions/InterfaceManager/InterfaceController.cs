using System.Windows;
using System.Windows.Controls;
using ClickQuest.Data;
using Microsoft.CSharp.RuntimeBinder;

namespace ClickQuest.Extensions.InterfaceManager
{
	public static class InterfaceController
	{
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

		// public static void RefreshStatPanels()
		// {
		// 	foreach (var page in GameData.Pages)
		// 	{
		// 		try
		// 		{
		// 			dynamic p = page.Value;
		// 			p.StatsFrame.Refresh();
		// 		}
		// 		catch (RuntimeBinderException)
		// 		{
		// 		}
		// 	}
		// }

		// public static void RefreshEquipmentPanels()
		// {
		// 	foreach (var page in GameData.Pages)
		// 	{
		// 		try
		// 		{
		// 			dynamic p = page.Value;
		// 			p.EquipmentFrame.Refresh();
		// 		}
		// 		catch (RuntimeBinderException)
		// 		{
		// 		}
		// 	}
		// }

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