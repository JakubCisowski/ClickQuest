using System.Windows;
using System.Windows.Media;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Extensions.UserInterface
{
	public static class ColorsController
	{
		public static SolidColorBrush GetRarityColor(Rarity rarity)
		{
			SolidColorBrush brush = null;

			switch (rarity)
			{
				case Rarity.General:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushRarity0");
					break;

				case Rarity.Fine:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushRarity1");
					break;

				case Rarity.Superior:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushRarity2");
					break;

				case Rarity.Exceptional:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushRarity3");
					break;

				case Rarity.Masterwork:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushRarity4");
					break;

				case Rarity.Mythic:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushRarity5");
					break;
			}

			return brush;
		}

		public static SolidColorBrush GetFloatingCombatTextColor(DamageType damageType)
		{
			SolidColorBrush brush = null;

			switch (damageType)
			{
				case DamageType.Normal:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushDamageTypeNormal");
					break;

				case DamageType.Critical:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushDamageTypeCritical");
					break;

				case DamageType.Poison:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushDamageTypePoison");
					break;

				case DamageType.Aura:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushDamageTypeAura");
					break;

				case DamageType.OnHit:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushDamageTypeOnHit");
					break;

				case DamageType.Artifact:
					brush = (SolidColorBrush) Application.Current.FindResource("BrushDamageTypeArtifact");
					break;
			}

			return brush;
		}
	}
}