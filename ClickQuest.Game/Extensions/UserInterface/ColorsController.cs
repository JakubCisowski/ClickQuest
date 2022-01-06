using System;
using System.Windows;
using System.Windows.Media;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Places;
using ClickQuest.Game.Extensions.Combat;
using ClickQuest.Game.UserInterface.Controls.Styles.Themes;

namespace ClickQuest.Game.Extensions.UserInterface;

public static class ColorsController
{
	public static SolidColorBrush GetRarityColor(Rarity rarity)
	{
		SolidColorBrush brush = null;

		switch (rarity)
		{
			case Rarity.General:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity0");
				break;

			case Rarity.Fine:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity1");
				break;

			case Rarity.Superior:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity2");
				break;

			case Rarity.Exceptional:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity3");
				break;

			case Rarity.Masterwork:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity4");
				break;

			case Rarity.Mythic:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity5");
				break;
		}

		return brush;
	}

	public static SolidColorBrush GetDamageTypeColor(DamageType damageType)
	{
		SolidColorBrush brush = null;

		switch (damageType)
		{
			case DamageType.Normal:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushFloatingTextNormal");
				break;

			case DamageType.Critical:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeCritical");
				break;

			case DamageType.Poison:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypePoison");
				break;

			case DamageType.Aura:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeAura");
				break;

			case DamageType.OnHit:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeOnHit");
				break;

			case DamageType.Artifact:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushDamageTypeArtifact");
				break;
		}

		return brush;
	}

	public static SolidColorBrush GetHeroClassColor(HeroClass heroClass)
	{
		SolidColorBrush brush = null;

		switch (heroClass)
		{
			case HeroClass.Slayer:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushSlayerRelated");
				break;

			case HeroClass.Venom:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushVenomRelated");
				break;
		}

		return brush;
	}

	public static SolidColorBrush GetMonsterSpawnRarityColor(MonsterSpawnPattern spawnPattern)
	{
		SolidColorBrush brush = null;

		var monsterSpawnFrequency = spawnPattern.Frequency;

		switch (monsterSpawnFrequency)
		{
			case <= 0.01:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity5");
				break;

			case <= 0.03:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity4");
				break;

			case <= 0.05:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity3");
				break;

			case <= 0.10:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity2");
				break;

			case <= 0.25:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity1");
				break;

			case > 0.25:
				brush = (SolidColorBrush)Application.Current.FindResource("BrushRarity0");
				break;
		}

		return brush;
	}

	public static void ChangeApplicationColorTheme(ColorTheme newTheme)
	{
		switch (newTheme)
		{
			case ColorTheme.Blue:
			{
				const string blueThemeXamlPath = "/UserInterface/Controls/Styles/Themes/Blue.xaml";

				var newDict = new ResourceDictionary
				{
					Source = new Uri(blueThemeXamlPath, UriKind.Relative)
				};

				foreach (var mergedDict in newDict.MergedDictionaries)
				{
					Application.Current.Resources.MergedDictionaries.Add(mergedDict);
				}

				foreach (var key in newDict.Keys)
				{
					Application.Current.Resources[key] = newDict[key];
				}

				break;
			}

			case ColorTheme.Orange:
			{
				const string orangeThemeXamlPath = "/UserInterface/Controls/Styles/Themes/Orange.xaml";

				var newDict = new ResourceDictionary
				{
					Source = new Uri(orangeThemeXamlPath, UriKind.Relative)
				};

				foreach (var mergedDict in newDict.MergedDictionaries)
				{
					Application.Current.Resources.MergedDictionaries.Add(mergedDict);
				}

				foreach (var key in newDict.Keys)
				{
					Application.Current.Resources[key] = newDict[key];
				}

				break;
			}

			case ColorTheme.Pink:
			{
				const string pinkThemeXamlPath = "/UserInterface/Controls/Styles/Themes/Pink.xaml";

				var newDict = new ResourceDictionary
				{
					Source = new Uri(pinkThemeXamlPath, UriKind.Relative)
				};

				foreach (var mergedDict in newDict.MergedDictionaries)
				{
					Application.Current.Resources.MergedDictionaries.Add(mergedDict);
				}

				foreach (var key in newDict.Keys)
				{
					Application.Current.Resources[key] = newDict[key];
				}

				break;
			}
		}

		GameAssets.RefreshPages();
	}
}