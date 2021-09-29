using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ClickQuest.Game.Extensions.Combat;
using MaterialDesignThemes.Wpf;
using static ClickQuest.Game.Extensions.Randomness.RandomnessController;
using Colors = ClickQuest.Game.Extensions.Interface.ColorsController;

namespace ClickQuest.Game.Extensions.Interface
{
	public static class FloatingTextController
	{
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

		public static (double X, double Y) RandomizeFloatingTextPathPosition(Point mousePosition, double canvasActualWidth, double canvasActualHeight, int maximumPositionOffset)
		{
			double randomizedPositionX = mousePosition.X + RNG.Next(-maximumPositionOffset, maximumPositionOffset + 1);
			double randomizedPositionY = mousePosition.Y + RNG.Next(-maximumPositionOffset, maximumPositionOffset + 1);

			// Clamp the positions, so that floating numbers do not follow cursor when user hovers over stats panel, equipment panel, etc.
			randomizedPositionX = Math.Clamp(randomizedPositionX, -90, canvasActualWidth + 45);
			randomizedPositionY = Math.Clamp(randomizedPositionY, -115, canvasActualHeight + 90);

			return (randomizedPositionX, randomizedPositionY);
		}

		public static Border CreateFloatingTextPanel(int damageValue, DamageType damageType)
		{
			var border = new Border
			{
				CornerRadius = new CornerRadius(20),
				BorderThickness = new Thickness(5),
				Background = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255)),
				Padding = new Thickness(2)
			};

			var stackPanel = new StackPanel
			{
				Orientation = Orientation.Horizontal
			};

			var textBrush = Colors.GetFloatingCombatTextColor(damageType);

			var icon = GetFloatingCombatIcon(damageType);
			icon.Foreground = textBrush;
			icon.VerticalAlignment = VerticalAlignment.Center;

			stackPanel.Children.Add(icon);

			var text = new TextBlock
			{
				Text = damageValue.ToString(),
				Foreground = textBrush,
				FontSize = 28,
				VerticalAlignment = VerticalAlignment.Center
			};

			if (damageType == DamageType.Critical)
			{
				text.FontWeight = FontWeights.DemiBold;
			}

			stackPanel.Children.Add(text);

			border.Child = stackPanel;

			return border;
		}

		public static PackIcon GetFloatingCombatIcon(DamageType damageType)
		{
			var icon = new PackIcon
			{
				Width = 28,
				Height = 28,
				VerticalAlignment = VerticalAlignment.Center
			};

			switch (damageType)
			{
				case DamageType.Normal:
					icon.Kind = PackIconKind.CursorDefault;
					break;

				case DamageType.Critical:
					icon.Kind = PackIconKind.CursorDefaultClick;
					break;

				case DamageType.Poison:
					icon.Kind = PackIconKind.Water;
					break;

				case DamageType.Aura:
					icon.Kind = PackIconKind.Brightness5;
					break;

				case DamageType.OnHit:
					icon.Kind = PackIconKind.CursorDefaultOutline;
					break;

				case DamageType.Artifact:
					icon.Kind = PackIconKind.DiamondStone;
					break;
			}

			return icon;
		}
	}
}