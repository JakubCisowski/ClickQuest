using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ClickQuest.Data;
using ClickQuest.Extensions.CollectionsManager;
using ClickQuest.Extensions.InterfaceManager;
using ClickQuest.Player;

namespace ClickQuest.Items
{
	public class Recipe : Item
	{
		[NotMapped]
		public List<Ingredient> Ingredients { get; set; }

		[NotMapped]
		public string RequirementsDescription { get; private set; }

		[NotMapped]
		public int IngotsRequired
		{
			get
			{
				return (int) (Rarity + 1) * 1000;
			}
		}

		public int ArtifactId { get; set; }

		public Artifact Artifact
		{
			get
			{
				return GameData.Artifacts.FirstOrDefault(x => x.Id == ArtifactId);
			}
		}

		public void UpdateRequirementsDescription()
		{
			RequirementsDescription = "Materials required: ";

			foreach (var ingredient in Ingredients)
			{
				var relatedMaterial = ingredient.RelatedMaterial;
				RequirementsDescription += relatedMaterial.Name + ": " + relatedMaterial.Value + "; ";
			}
		}

		public void UpdateDescription()
		{
			Description = GameData.Artifacts.FirstOrDefault(x => x.Id == ArtifactId)?.Description;
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
			InterfaceController.RefreshEquipmentPanels();
		}

		public override void RemoveItem(int amount = 1)
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Recipes);
		}
	}
}