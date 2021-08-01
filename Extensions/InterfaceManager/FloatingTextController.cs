using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ClickQuest.Extensions.CombatManager;
using Colors = ClickQuest.Styles.Colors;

namespace ClickQuest.Extensions.InterfaceManager
{
	public static class FloatingTextController
	{
		private static readonly Random _rng = new Random();

		public static DoubleAnimation CreateTextOpacityAnimation(int durationInSeconds)
		{
			var textVisibilityAnimation = new DoubleAnimation
			{
				Name = "ClickTextVisibilityAnimation",
				Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds)),
				From = 1.0,
				To = 0.0
			};

			return textVisibilityAnimation;
		}

		public static Path CreateFloatingTextPath(int damage, DamageType damageType, double baseTextSize)
		{
			// Source: https://stackoverflow.com/questions/4559485/wpf-animation-is-not-smooth
			var path = new Path();

			var textBrush = Colors.GetFloatingCombatTextColor(damageType);
			
			var formattedText = new FormattedText(damage.ToString(), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Algerian"), baseTextSize, Brushes.Black);
			
			if (damageType == DamageType.Critical)
			{
				formattedText.SetFontWeight(FontWeights.DemiBold);
			}
			
			// (0,0) because we'll modify the position of path anyway
			path.Data = formattedText.BuildGeometry(new Point(0,0));
			path.Fill = textBrush;

			return path;
		}

		public static (double X, double Y) RandomizeFloatingTextPathPosition(Point mousePosition, double canvasActualWidth, double canvasActualHeight, int maximumPositionOffset)
		{
			double randomizedPositionX = mousePosition.X + _rng.Next(-maximumPositionOffset, maximumPositionOffset + 1);
			double randomizedPositionY = mousePosition.Y + _rng.Next(-maximumPositionOffset, maximumPositionOffset + 1);

			// Clamp the positions, so that floating numbers do not follow cursor when user hovers over stats panel, equipment panel, etc.
			randomizedPositionX = Math.Clamp(randomizedPositionX, -maximumPositionOffset, canvasActualWidth + maximumPositionOffset);
			randomizedPositionY = Math.Clamp(randomizedPositionY, -maximumPositionOffset, canvasActualHeight + maximumPositionOffset);

			return (randomizedPositionX, randomizedPositionY);
		}
	}
}