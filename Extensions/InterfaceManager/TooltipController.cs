using ClickQuest.Data;
using ClickQuest.Items;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ClickQuest.Extensions.InterfaceManager
{
	public static class TooltipController
	{
		public static ToolTip GenerateEquipmentItemTooltip<T>(T itemToGenerateTooltipFor) where T:Item
		{
			var toolTip = new ToolTip();
			

			var toolTipBlock = new TextBlock()
			{
				Style = (Style)Application.Current.FindResource("ToolTipTextBlockBase")
			};

			switch (itemToGenerateTooltipFor)
			{
				case Material material:
				{
					toolTipBlock.Inlines.Add(new Run($"{material.Name}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"*{material.RarityString}*") { Foreground = Styles.Colors.GetRarityColor(material.Rarity), FontWeight = FontWeights.DemiBold });
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{material.Description}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"Value: {material.Value} gold"));
				}
					break;

				case Artifact artifact:
				{
					toolTipBlock.Inlines.Add(new Run($"{artifact.Name}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"*{artifact.RarityString}*") { Foreground = Styles.Colors.GetRarityColor(artifact.Rarity), FontWeight = FontWeights.DemiBold });
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{artifact.Description}"));
				}
					break;
					
				case Recipe recipe:
				{
					toolTipBlock.Inlines.Add(new Run($"{recipe.Name}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"*{recipe.RarityString}*") { Foreground = Styles.Colors.GetRarityColor(recipe.Rarity), FontWeight = FontWeights.DemiBold });
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{recipe.Description}"));
					toolTipBlock.Inlines.Add(new LineBreak());
					toolTipBlock.Inlines.Add(new Run($"{recipe.RequirementsDescription}"));
				}
					break;
			}

			toolTip.Content = toolTipBlock;

			return toolTip;
		} 

		public static void SetTooltipDelayAndDuration(DependencyObject control)
		{
			ToolTipService.SetInitialShowDelay(control, 100);
			ToolTipService.SetShowDuration(control, 20000);
		}
	}
}