using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Functionalities;
using ClickQuest.Game.UserInterface.Controls;
using ClickQuest.Game.UserInterface.Helpers;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Game.Models;

public class Artifact : Item
{
	[JsonIgnore]
	public ArtifactFunctionality ArtifactFunctionality { get; set; }

	public ArtifactType ArtifactType { get; set; }
	public string Lore { get; set; }
	public string ExtraInfo { get; set; }
	public string MythicTag { get; set; }
	public const double MeltingIngredientsRatio = 0.6;
	public const int MeltingWithoutIngredientsValue = 120;
	public const double CraftingRatio = 25;

	public int BaseIngotBonus => 100;

	public override Artifact CopyItem(int quantity)
	{
		var copy = new Artifact
		{
			Id = Id,
			Name = Name,
			Rarity = Rarity,
			Value = Value,
			Quantity = quantity,
			ArtifactType = ArtifactType,
			ArtifactFunctionality = ArtifactFunctionality,
			Description = Description,
			Lore = Lore,
			ExtraInfo = ExtraInfo,
			MythicTag = MythicTag
		};

		return copy;
	}

	public override void AddAchievementProgress(int amount = 1)
	{
		NumericAchievementType achievementType = 0;
		// Increase achievement amount.
		switch (Rarity)
		{
			case Rarity.General:
				achievementType = NumericAchievementType.GeneralArtifactsGained;
				break;
			case Rarity.Fine:
				achievementType = NumericAchievementType.FineArtifactsGained;
				break;
			case Rarity.Superior:
				achievementType = NumericAchievementType.SuperiorArtifactsGained;
				break;
			case Rarity.Exceptional:
				achievementType = NumericAchievementType.ExceptionalArtifactsGained;
				break;
			case Rarity.Mythic:
				achievementType = NumericAchievementType.MythicArtifactsGained;
				break;
			case Rarity.Masterwork:
				achievementType = NumericAchievementType.MasterworkArtifactsGained;
				break;
		}

		User.Instance.Achievements.IncreaseAchievementValue(achievementType, amount);
	}

	public override void AddItem(int amount = 1, bool displayFloatingText = false)
	{
		CollectionsHelper.AddItemToCollection(this, User.Instance.CurrentHero.Artifacts, amount);

		if (displayFloatingText)
		{
			LootQueueHelper.AddToQueue(Name, Rarity, PackIconKind.DiamondStone);
		}

		AddAchievementProgress();
		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Artifact));
	}

	public override void RemoveItem(int amount = 1)
	{
		CollectionsHelper.RemoveItemFromCollection(this, User.Instance.CurrentHero.Artifacts, amount);

		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Artifact));
		
		if (User.Instance.CurrentHero.ArtifactSets.Any(x => x.ArtifactIds.Any(y => y == Id)) && !User.Instance.CurrentHero.Artifacts.Contains(this))
		{
			// Special case - if Ammunition was removed (for example by consuming it in combat), do not show the popup.
			if (this.ArtifactType != ArtifactType.Ammunition)
			{
				AlertBox.Show($"{Name} has been removed from all Artifact Sets", MessageBoxButton.OK);
			}
			
			foreach (var artifactSet in User.Instance.CurrentHero.ArtifactSets)
			{
				artifactSet.ArtifactIds.Remove(Id);
			}
		}
	}

	public void CreateMythicTag(string enemyName = "")
	{
		if (Rarity == Rarity.Mythic)
		{
			if (enemyName == "")
			{
				MythicTag = "Crafted by " + User.Instance.CurrentHero.Name + " on " + DateTime.Now.ToString("dd/MM/yyyy");
			}
			else
			{
				MythicTag = "Dropped from " + enemyName + " by " + User.Instance.CurrentHero.Name + " on " + DateTime.Now.ToString("dd/MM/yyyy");
			}
		}
	}
}