using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClickQuest.Game.Models;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Pages;
using ClickQuest.Game.UserInterface.Windows;
using Microsoft.CSharp.RuntimeBinder;

namespace ClickQuest.Game.UserInterface.Helpers;

public static class InterfaceHelper
{
	public const int EquipmentItemsNeededToShowScrollBar = 15;
	public const int EquipmentItemsNeededToShowScrollBarIfArtifactsAreEquipped = 10;
	public const int VendorItemsNeededToShowScrollBar = 12;

	public static Enemy CurrentEnemy
	{
		get
		{
			var currentFrameContent = (Application.Current.MainWindow as GameWindow)?.CurrentFrame.Content;
			var isCurrentFrameContentARegion = currentFrameContent is RegionPage;

			if (isCurrentFrameContentARegion)
			{
				return (currentFrameContent as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault()?.Monster;
			}

			var isCurrentFrameContentADungeon = currentFrameContent is DungeonBossPage;

			if (isCurrentFrameContentADungeon)
			{
				return (currentFrameContent as DungeonBossPage).Boss;
			}

			return null;
		}
	}

	public static MonsterButton CurrentMonsterButton
	{
		get
		{
			var isCurrentFrameContentARegion = GameAssets.CurrentPage is RegionPage;

			if (isCurrentFrameContentARegion)
			{
				return (GameAssets.CurrentPage as RegionPage).RegionPanel.Children.OfType<MonsterButton>().FirstOrDefault();
			}

			return null;
		}
	}

	public static DungeonBossPage CurrentBossPage
	{
		get
		{
			var isCurrentFrameContentARegion = GameAssets.CurrentPage is DungeonBossPage;

			if (isCurrentFrameContentARegion)
			{
				return GameAssets.CurrentPage as DungeonBossPage;
			}

			return null;
		}
	}

	public static void RefreshStatsAndEquipmentPanelsOnCurrentPage()
	{
		try
		{
			dynamic p = GameAssets.CurrentPage;

			HeroStatsPage statsPage = p.StatsFrame.Content;
			statsPage.RefreshAllDynamicStatsAndToolTips();

			EquipmentPage equipmentPage = p.EquipmentFrame.Content;
			equipmentPage.ChangeActiveTab();
			equipmentPage.RefreshCurrentEquipmentTab();
		}
		catch (RuntimeBinderException)
		{
			// No stats frame on this page!
			// Best solution according to:
			// https://stackoverflow.com/a/5768449/14770235
		}
		catch (NullReferenceException)
		{
			// This might be necessary instead of the above with the new approach.
		}
	}

	public static void RefreshSpecificEquipmentPanelTabOnCurrentPage(Type itemType)
	{
		try
		{
			dynamic p = GameAssets.CurrentPage;

			EquipmentPage equipmentPage = p.EquipmentFrame.Content;
			equipmentPage.RefreshSpecificEquipmentTab(itemType);
		}
		catch (RuntimeBinderException)
		{
			// No stats frame on this page!
			// Best solution according to:
			// https://stackoverflow.com/a/5768449/14770235
		}
		catch (NullReferenceException)
		{
			// This might be necessary instead of the above with the new approach.
		}
	}

	public static void RefreshCurrentEquipmentPanelTabOnCurrentPage()
	{
		try
		{
			dynamic p = GameAssets.CurrentPage;

			EquipmentPage equipmentPage = p.EquipmentFrame.Content;
			equipmentPage.RefreshCurrentEquipmentTab();
		}
		catch (RuntimeBinderException)
		{
			// No stats frame on this page!
			// Best solution according to:
			// https://stackoverflow.com/a/5768449/14770235
		}
		catch (NullReferenceException)
		{
			// This might be necessary instead of the above with the new approach.
		}
	}

	public static void UpdateSingleSpecializationInterface(SpecializationType specializationType)
	{
		try
		{
			dynamic p = GameAssets.CurrentPage;

			HeroStatsPage statsPage = p.StatsFrame.Content;
			statsPage.UpdateSingleSpecializationInterface(specializationType);
		}
		catch (RuntimeBinderException)
		{
			// No stats frame on this page!
			// Best solution according to:
			// https://stackoverflow.com/a/5768449/14770235
		}
		catch (NullReferenceException)
		{
			// This might be necessary instead of the above with the new approach.
		}
	}

	public static void RefreshBlessingInterfaceOnCurrentPage(BlessingType blessingType)
	{
		try
		{
			dynamic p = GameAssets.CurrentPage;

			HeroStatsPage statsPage = p.StatsFrame.Content;
			statsPage.UpdateBlessingTimer();

			switch (blessingType)
			{
				case BlessingType.ClickDamage:
					statsPage.GenerateStatValueDamageToolTip();
					break;

				case BlessingType.CritChance or BlessingType.CritDamage:
					statsPage.GenerateStatValueCritToolTip();
					break;

				case BlessingType.PoisonDamage:
					statsPage.GenerateStatValuePoisonToolTip();
					break;

				case BlessingType.AuraDamage or BlessingType.AuraSpeed:
					statsPage.GenerateStatValueAuraToolTip();
					break;
			}
		}
		catch (RuntimeBinderException)
		{
			// No stats frame on this page!
			// Best solution according to:
			// https://stackoverflow.com/a/5768449/14770235
		}
		catch (NullReferenceException)
		{
			// This might be necessary instead of the above with the new approach.
		}
	}

	public static void RefreshQuestInterfaceOnCurrentPage()
	{
		try
		{
			dynamic p = GameAssets.CurrentPage;

			HeroStatsPage statsPage = p.StatsFrame.Content;
			statsPage.UpdateQuestTimer();
		}
		catch (RuntimeBinderException)
		{
			// No stats frame on this page!
			// Best solution according to:
			// https://stackoverflow.com/a/5768449/14770235
		}
		catch (NullReferenceException)
		{
			// This might be necessary instead of the above with the new approach.
		}
	}

	public static void ChangePage(Page destinationPage, string locationInfoText)
	{
		try
		{
			dynamic p = GameAssets.CurrentPage;
			(p.EquipmentFrame.Content as EquipmentPage).SaveScrollbarOffset();
		}
		catch (RuntimeBinderException)
		{
		}

		var window = Application.Current.MainWindow as GameWindow;
		window.CurrentFrame.Navigate(destinationPage);
		window.LocationInfo = locationInfoText;

		GameAssets.CurrentPage = destinationPage;
		RefreshStatsAndEquipmentPanelsOnCurrentPage();
		
		// Putting these here so that they're always called in one place.
		
		// If switching to Town, make sure to re-make Region Buttons (in case the player has leveled up), otherwise they couldn't access new zones.
		if (destinationPage is TownPage townPage)
		{
			townPage.GenerateRegionButtons();
		}
		// If switching to Main Menu, make sure to re-make Hero Buttons (to reflect the change in hero level as well as time played).
		// Also, update Time Played (otherwise it's only updated when exiting the game).
		else if (destinationPage is MainMenuPage menuPage)
		{
			menuPage.UpdateSelectOrDeleteHeroButtons();
			
			User.Instance.Achievements.TotalTimePlayed += DateTime.Now - User.SessionStartDate;
			User.SessionStartDate = DateTime.Now;
		}
	}
}