using System.Windows;
using System.Windows.Media;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Items;

namespace ClickQuest.Styles
{
	public static class Colors
	{
		public static SolidColorBrush GetRarityColor(Rarity rarity)
		{
			SolidColorBrush brush = null;

			switch (rarity)
			{
				case Rarity.General:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorRarity0");
					break;

				case Rarity.Fine:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorRarity1");
					break;

				case Rarity.Superior:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorRarity2");
					break;

				case Rarity.Exceptional:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorRarity3");
					break;

				case Rarity.Mythic:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorRarity4");
					break;

				case Rarity.Masterwork:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorRarity5");
					break;
			}

			return brush;
		}

		public static SolidColorBrush GetFloatingCombatTextColor(FloatingTextController.DamageType damageType)
		{
			SolidColorBrush brush = null;

			switch (damageType)
			{
				case FloatingTextController.DamageType.Normal:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorDamageTypeNormal");
					break;
				
				case FloatingTextController.DamageType.Critical:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorDamageTypeCritical");
					break;
				
				case FloatingTextController.DamageType.Poison:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorDamageTypePoison");
					break;
				
				case FloatingTextController.DamageType.Aura:
					brush = (SolidColorBrush) Application.Current.FindResource("ColorDamageTypeAura");
					break;
			}

			return brush;
		}
	}
}