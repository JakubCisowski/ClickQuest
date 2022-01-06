using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.UserInterface.Helpers;

namespace ClickQuest.Game.Models;

public class Recipe : Item
{
	[JsonIgnore]
	public override string Description { get; set; }

	public string FullName => "Recipe: " + Name;

	public List<IngredientPattern> IngredientPatterns { get; set; }

	public string RequirementsDescription { get; private set; }

	public int ArtifactId { get; set; }

	public Artifact Artifact
	{
		get
		{
			return GameAssets.Artifacts.FirstOrDefault(x => x.Id == ArtifactId);
		}
	}

	public void UpdateRequirementsDescription()
	{
		IngredientPatterns = GameAssets.Recipes.FirstOrDefault(x => x.Id == Id)?.IngredientPatterns;

		RequirementsDescription = "Materials required:";

		foreach (var ingredient in IngredientPatterns)
		{
			var relatedMaterial = ingredient.RelatedMaterial;
			RequirementsDescription += $"\n- {ingredient.Quantity}x {relatedMaterial.Name}";
		}
	}

	public void UpdateDescription()
	{
		Description = GameAssets.Artifacts.FirstOrDefault(x => x.Id == ArtifactId)?.Description;
	}

	public override Recipe CopyItem(int quantity)
	{
		var copy = new Recipe
		{
			Id = Id,
			Name = Name,
			Rarity = Rarity,
			Value = Value,
			Description = Description,
			Quantity = quantity,
			ArtifactId = ArtifactId,
			IngredientPatterns = IngredientPatterns,
			RequirementsDescription = RequirementsDescription
		};

		return copy;
	}

	public override void AddAchievementProgress(int amount = 1)
	{
		User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.RecipesGained, amount);
	}

	public override void AddItem(int amount = 1)
	{
		CollectionsHelper.AddItemToCollection(this, User.Instance.CurrentHero.Recipes, amount);

		AddAchievementProgress();
		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Recipe));
	}

	public override void RemoveItem(int amount = 1)
	{
		CollectionsHelper.RemoveItemFromCollection(this, User.Instance.CurrentHero.Recipes, amount);

		InterfaceHelper.RefreshSpecificEquipmentPanelTabOnCurrentPage(typeof(Recipe));
	}
}