using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ClickQuest.Extensions.CombatManager;
using ClickQuest.Styles;

namespace ClickQuest.Extensions.InterfaceManager
{
	public static class FloatingTextController
	{
		private static readonly Random _rng = new Random();

		public static DoubleAnimation CreateTextSizeAnimation(int durationInSeconds, double baseTextSize)
		{
			var textSizeAnimation = new DoubleAnimation
			{
				Name = "ClickTextSizeAnimation",
				Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds)),
				From = baseTextSize,
				To = baseTextSize / 2d
			};

			return textSizeAnimation;
		}

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

		public static TextBlock CreateFloatingTextBlock(int damage, DamageType damageType, double baseTextSize)
		{
			var damageBlock = new TextBlock
			{
				Text = damage.ToString(),
				FontSize = baseTextSize,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			damageBlock.Foreground = Colors.GetFloatingCombatTextColor(damageType);

			// Todo: add more styles (such as borders around crits, icons next to poison, etc)
			if (damageType == DamageType.Critical)
			{
				damageBlock.FontWeight = FontWeights.DemiBold;
			}

			return damageBlock;
		}

		public static (double X, double Y) RandomizeFloatingTextBlockPosition(Point mousePosition, double canvasActualWidth, double canvasActualHeight, int maximumPositionOffset)
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