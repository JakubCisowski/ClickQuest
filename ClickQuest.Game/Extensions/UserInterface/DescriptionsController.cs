using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ClickQuest.Game.Core.Heroes;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Extensions.Combat;

namespace ClickQuest.Game.Extensions.UserInterface
{
	public static class DescriptionsController
	{
		private const string TagOpeningStart = "<";
		private const string TagClosingStart = "</";
		private const string TagEnd = ">";

		public static List<Run> GenerateDescriptionRuns(string description)
		{
			var descriptionRuns = new List<Run>();

			// If it's called before loading descriptions.
			if (description is null)
			{
				return descriptionRuns;
			}

			while (true)
			{
				int indexOfTagOpeningStart = description.IndexOf(TagOpeningStart);

				if (indexOfTagOpeningStart == -1)
				{
					// Tag opening not found - create a normal Run with the remainder of description.
					descriptionRuns.Add(new Run(description));
					break;
				}

				if (indexOfTagOpeningStart != 0)
				{
					// If tag opening index is not zero, first create a normal Run with that part of description.
					string taglessPart = description.Substring(0, indexOfTagOpeningStart);
					descriptionRuns.Add(new Run(taglessPart));
					description = description.Remove(0, indexOfTagOpeningStart);
					indexOfTagOpeningStart = 0;
				}

				// Find closing tag.
				int indexOfTagOpeningEnd = description.IndexOf(TagEnd);
				int indexOfTagClosingStart = description.IndexOf(TagClosingStart);

				string tagType = description.Substring(1, indexOfTagOpeningEnd - indexOfTagOpeningStart - 1).ToUpper();

				string taggedPart = description.Substring(indexOfTagOpeningEnd + 1, indexOfTagClosingStart - indexOfTagOpeningEnd - 1);

				Run coloredRun = new Run(taggedPart);

				switch (tagType)
				{
					// Damage type
					case "NORMAL":
						coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Normal);
						break;
					case "CRITICAL":
						coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Critical);
						break;
					case "POISON":
						coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Poison);
						break;
					case "AURA":
						coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Aura);
						break;
					case "ONHIT":
						coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.OnHit);
						break;
					case "ARTIFACT":
						coloredRun.Foreground = ColorsController.GetDamageTypeColor(DamageType.Artifact);
						break;

					// Class 
					case "SLAYER":
						coloredRun.Foreground = ColorsController.GetHeroClassColor(HeroClass.Slayer);
						break;
					case "VENOM":
						coloredRun.Foreground = ColorsController.GetHeroClassColor(HeroClass.Venom);
						break;

					// Rarity
					case "GENERAL":
						coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.General);
						break;
					case "FINE":
						coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Fine);
						break;
					case "SUPERIOR":
						coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Superior);
						break;
					case "EXCEPTIONAL":
						coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Exceptional);
						break;
					case "MASTERWORK":
						coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Masterwork);
						break;
					case "MYTHIC":
						coloredRun.Foreground = ColorsController.GetRarityColor(Rarity.Mythic);
						break;

					// Font style
					case "BOLD":
						coloredRun.FontFamily = (FontFamily) Application.Current.FindResource("FontRegularDemiBold");
						break;
				}

				descriptionRuns.Add(coloredRun);

				// Remove opening tag, tagged part and closing tag from description.
				description = description.Remove(0, indexOfTagClosingStart);
				int indexOfTagClosingEnd = description.IndexOf(TagEnd);
				description = description.Remove(0, indexOfTagClosingEnd + 1);
			}

			return descriptionRuns;
		}
	}
}