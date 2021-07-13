using ClickQuest.Data;
using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;

namespace ClickQuest.Extensions.InterfaceManager
{
	public static class InterfaceController
	{
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
					// No stats frame on this page!
					// Best solution according to:
					// https://stackoverflow.com/a/5768449/14770235
				} 
			}
		}
	}
}