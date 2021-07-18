using ClickQuest.Data;
using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;
using System.Windows.Controls;

namespace ClickQuest.Extensions.InterfaceManager
{
	public static class InterfaceController
	{
		public static void RefreshStatsAndEquipmentPanelsOnPage(Page pageToRefresh)
		{
			try
			{
				dynamic p = pageToRefresh;
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
		
		public static void RefreshStatPanels()
		{
			foreach (var page in GameData.Pages)
			{
				try
				{
					dynamic p = page.Value;
					p.StatsFrame.Refresh();
				}
				catch (RuntimeBinderException)
				{
				} 
			}
		}

		public static void RefreshEquipmentPanels()
		{
			foreach (var page in GameData.Pages)
			{
				try
				{
					dynamic p = page.Value;
					p.EquipmentFrame.Refresh();
				}
				catch (RuntimeBinderException)
				{
				} 
			}
		}
	}
}