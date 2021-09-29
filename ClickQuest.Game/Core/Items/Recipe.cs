using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.Extensions.Interface;

namespace ClickQuest.Game.Core.Items
{
	public class Recipe : Item
	{
		public List<Ingredient> Ingredients { get; set; }

		public string RequirementsDescription { get; private set; }

		[JsonIgnore]
		public int IngotsRequired
		{
			get
			{
				return (int) (Rarity + 1) * 1000;
			}
		}

		public int ArtifactId { get; set; }

		[JsonIgnore]
		public Artifact Artifact
		{
			get
			{
				return GameAssets.Artifacts.FirstOrDefault(x => x.Id == ArtifactId);
			}
		}

		public void UpdateRequirementsDescription()
		{
			Ingredients = GameAssets.Recipes.FirstOrDefault(x => x.Id == Id)?.Ingredients;

			RequirementsDescription = "Materials required: ";

			foreach (var ingredient in Ingredients)
			{
				var relatedMaterial = ingredient.RelatedMaterial;
				RequirementsDescription += relatedMaterial.Name + ": " + relatedMaterial.Value + "; ";
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
			copy.Ingredients = Ingredients;
			copy.RequirementsDescription = RequirementsDescription;

			return copy;
		}

		public override void AddAchievementProgress(int amount = 1)
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.RecipesGained, amount);
		}

		public override void AddItem(int amount = 1)
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Recipes);

			AddAchievementProgress();
			InterfaceController.RefreshStatsAndEquipmentPanelsOnCurrentPage();
		}

		public override void RemoveItem(int amount = 1)
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Recipes);
		}
	}
}