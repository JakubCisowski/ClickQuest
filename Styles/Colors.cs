using System.Windows;
using System.Windows.Media;
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
					brush = (SolidColorBrush)Application.Current.FindResource("ColorRarity0");
					break;
				
				case Rarity.Fine:
					brush = (SolidColorBrush)Application.Current.FindResource("ColorRarity1");
					break;
					
				case Rarity.Superior:
					brush = (SolidColorBrush)Application.Current.FindResource("ColorRarity2");
					break;
					
				case Rarity.Exceptional:
					brush = (SolidColorBrush)Application.Current.FindResource("ColorRarity3");
					break;
					
				case Rarity.Mythic:
					brush = (SolidColorBrush)Application.Current.FindResource("ColorRarity4");
					break;
					
				case Rarity.Masterwork:
					brush = (SolidColorBrush)Application.Current.FindResource("ColorRarity5");
					break;
			}

			return brush;
		}
	}
}