using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.UserInterface.Helpers;

public static class DescriptionsHelper
{
	private const string TagOpeningStart = "<";
	private const string TagClosingStart = "</";
	private const string TagEnd = ">";

	public static List<Inline> GenerateDescriptionInlines(string description, SolidColorBrush defaultBrush)
	{
		var descriptionInlines = new List<Inline>();

		// If it's called before loading descriptions.
		if (description is null)
		{
			return descriptionInlines;
		}

		while (true)
		{
			var indexOfTagOpeningStart = description.IndexOf(TagOpeningStart, StringComparison.Ordinal);

			if (indexOfTagOpeningStart == -1)
			{
				// Tag opening not found - create a normal Run with the remainder of description.
				descriptionInlines.Add(new Run(description));
				break;
			}

			if (indexOfTagOpeningStart != 0)
			{
				// If tag opening index is not zero, first create a normal Run with that part of description.
				var taglessPart = description.Substring(0, indexOfTagOpeningStart);
				descriptionInlines.Add(new Run(taglessPart));
				description = description.Remove(0, indexOfTagOpeningStart);
				indexOfTagOpeningStart = 0;
			}

			// Find closing tag.
			var indexOfTagOpeningEnd = description.IndexOf(TagEnd, StringComparison.Ordinal);
			var indexOfTagClosingStart = description.IndexOf(TagClosingStart, StringComparison.Ordinal);

			var tagType = description.Substring(1, indexOfTagOpeningEnd - indexOfTagOpeningStart - 1).ToUpper();

			if (tagType == "NEWLINE")
			{
				descriptionInlines.Add(new LineBreak());
			}
			else
			{
				var taggedPart = description.Substring(indexOfTagOpeningEnd + 1, indexOfTagClosingStart - indexOfTagOpeningEnd - 1);

				var coloredRun = new Run(taggedPart)
				{
					Foreground = defaultBrush
				};

				switch (tagType)
				{
					// Damage type
					case "NORMAL":
						coloredRun.Foreground = ColorsHelper.GetDamageTypeColor(DamageType.Normal);
						break;
					case "CRITICAL":
						coloredRun.Foreground = ColorsHelper.GetDamageTypeColor(DamageType.Critical);
						break;
					case "POISON":
						coloredRun.Foreground = ColorsHelper.GetDamageTypeColor(DamageType.Poison);
						break;
					case "AURA":
						coloredRun.Foreground = ColorsHelper.GetDamageTypeColor(DamageType.Aura);
						break;
					case "ONHIT":
						coloredRun.Foreground = ColorsHelper.GetDamageTypeColor(DamageType.OnHit);
						break;
					case "MAGIC":
						coloredRun.Foreground = ColorsHelper.GetDamageTypeColor(DamageType.Magic);
						break;

					// Class 
					case "SLAYER":
						coloredRun.Foreground = ColorsHelper.GetHeroClassColor(HeroClass.Slayer);
						break;
					case "VENOM":
						coloredRun.Foreground = ColorsHelper.GetHeroClassColor(HeroClass.Venom);
						break;

					// Rarity
					case "GENERAL":
						coloredRun.Foreground = ColorsHelper.GetRarityColor(Rarity.General);
						break;
					case "FINE":
						coloredRun.Foreground = ColorsHelper.GetRarityColor(Rarity.Fine);
						break;
					case "SUPERIOR":
						coloredRun.Foreground = ColorsHelper.GetRarityColor(Rarity.Superior);
						break;
					case "EXCEPTIONAL":
						coloredRun.Foreground = ColorsHelper.GetRarityColor(Rarity.Exceptional);
						break;
					case "MASTERWORK":
						coloredRun.Foreground = ColorsHelper.GetRarityColor(Rarity.Masterwork);
						break;
					case "MYTHIC":
						coloredRun.Foreground = ColorsHelper.GetRarityColor(Rarity.Mythic);
						break;

					// Font style
					case "BOLD":
						coloredRun.FontFamily = (FontFamily)Application.Current.FindResource("FontRegularDemiBold");
						break;
				}

				descriptionInlines.Add(coloredRun);
			}

			// Remove opening tag, tagged part and closing tag from description.
			description = description.Remove(0, indexOfTagClosingStart);
			var indexOfTagClosingEnd = description.IndexOf(TagEnd, StringComparison.Ordinal);
			description = description.Remove(0, indexOfTagClosingEnd + 1);
		}

		return descriptionInlines;
	}
}