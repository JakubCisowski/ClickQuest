﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ClickQuest.Game.Extensions.UserInterface.ToolTips
{
	public static class GeneralToolTipController
	{
		public static void SetToolTipDelayAndDuration(DependencyObject control)
		{
			ToolTipService.SetInitialShowDelay(control, 100);
			ToolTipService.SetShowDuration(control, 20000);
		}

		public static Line GenerateTextSeparator()
		{
			return new Line() { X1 = 0, Y1 = 0, X2 = 500, Stroke = (SolidColorBrush)Application.Current.FindResource("BrushGame3"), StrokeThickness = 1, Margin = new Thickness(0, 6, 0, 6) };
		}
	}
}