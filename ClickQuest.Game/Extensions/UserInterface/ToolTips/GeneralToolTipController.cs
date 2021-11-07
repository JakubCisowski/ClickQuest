using System.Windows;
using System.Windows.Controls;

namespace ClickQuest.Game.Extensions.UserInterface.ToolTips
{
	public static class GeneralToolTipController
	{
		public static void SetToolTipDelayAndDuration(DependencyObject control)
		{
			ToolTipService.SetInitialShowDelay(control, 100);
			ToolTipService.SetShowDuration(control, 20000);
		}
	}
}
