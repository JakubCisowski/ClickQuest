using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ClickQuest.Game.UserInterface.Helpers.ToolTips;

public static class GeneralToolTipHelper
{
	public static void SetToolTipDelayAndDuration(DependencyObject control)
	{
		ToolTipService.SetInitialShowDelay(control, 100);
		ToolTipService.SetShowDuration(control, 20000);
	}

	public static Line GenerateTextSeparator()
	{
		return new Line
		{
			X1 = 0,
			Y1 = 0,
			X2 = 500,
			Stroke = (SolidColorBrush)Application.Current.FindResource("BrushWhite"),
			StrokeThickness = 1,
			Margin = new Thickness(0, 6, 0, 6)
		};
	}

	public static ToolTip GenerateUndiscoveredEnemyToolTip()
	{
		var toolTip = new ToolTip
		{
			Style = (Style)Application.Current.FindResource("ToolTipSimple")
		};

		var toolTipBlock = new TextBlock
		{
			Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase"),
			Text = "You have not discovered this Enemy yet\nIt will show up here once you first fight it"
		};

		toolTip.Content = toolTipBlock;

		return toolTip;
	}
}