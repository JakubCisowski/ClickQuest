using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Items.Patterns;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.UserInterface;

namespace ClickQuest.Game.Core.Items
{
	public class Recipe : Item
	{
		[JsonIgnore]
		public override string Description { get; set; }
		public List<IngredientPattern> IngredientPatterns { get; set; }

		public string RequirementsDescription { get; private set; }

		public int ArtifactId { get; set; }

		public Artifact Artifact => GameAssets.Artifacts.FirstOrDefault(x => x.Id == ArtifactId);

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
			var copy = new Recipe();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = quantity;
			copy.ArtifactId = ArtifactId;
			copy.IngredientPatterns = IngredientPatterns;
			copy.RequirementsDescription = RequirementsDescription;

			return copy;
		}

		public override void AddAchievementProgress(int amount = 1)
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.RecipesGained, amount);
		}

		public override void AddItem(int amount = 1)
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Recipes, amount);

			AddAchievementProgress();
			InterfaceController.RefreshCurrentEquipmentPanelTabOnCurrentPage();
		}

		public override void RemoveItem(int amount = 1)
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Recipes, amount);
			
			InterfaceController.RefreshCurrentEquipmentPanelTabOnCurrentPage();
		}
	}
}