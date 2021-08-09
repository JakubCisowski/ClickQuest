using System.Windows;
using System.Windows.Controls;
using ClickQuest.Data.GameData;
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