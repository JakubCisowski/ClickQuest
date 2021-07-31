using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ClickQuest.Extensions.CombatManager
{
	public static class FloatingTextController
	{
		public enum DamageType
		{
			Normal, Critical, Poison, Aura
		}
		public static DoubleAnimation CreateTextSizeAnimation(int durationInSeconds, double baseTextSize)
		{
			var textSizeAnimation = new DoubleAnimation()
			{
				Name = "ClickTextSizeAnimation",
				Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds)),
				From = baseTextSize,
				To = baseTextSize/2d
			};

			return textSizeAnimation;
		}

		public static DoubleAnimation CreateTextOpacityAnimation(int durationInSeconds)
		{
			var textVisibilityAnimation = new DoubleAnimation()
			{
				Name = "ClickTextVisibilityAnimation",
				Duration = new Duration(TimeSpan.FromSeconds(durationInSeconds)),
				From = 1.0,
				To = 0.0
			};

			return textVisibilityAnimation;
		}
	}
}