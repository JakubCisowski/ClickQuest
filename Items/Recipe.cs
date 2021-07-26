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
		// Key is the Id of the material; value is the amount needed.
		[NotMapped]
		public Dictionary<int, int> MaterialIds { get; set; }

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
			MaterialIds = GameData.Recipes.FirstOrDefault(x => x.Id == Id)?.MaterialIds;

			RequirementsDescription = "Materials required: ";

			foreach (var elem in MaterialIds)
			{
				// Add name
				RequirementsDescription += GameData.Materials.FirstOrDefault(x => x.Id == elem.Key).Name;
				// Add quantity
				RequirementsDescription = RequirementsDescription + ": " + elem.Value + "; ";
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
			copy.MaterialIds = MaterialIds;
			copy.RequirementsDescription = RequirementsDescription;

			return copy;
		}

		public override void AddAchievementProgress(int amount)
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.RecipesGained, amount);
		}

		public override void AddItem()
		{
			CollectionsController.AddItemToCollection(this, User.Instance.CurrentHero.Recipes);

			AddAchievementProgress(1);
			InterfaceController.RefreshEquipmentPanels();
		}

		public override void RemoveItem()
		{
			CollectionsController.RemoveItemFromCollection(this, User.Instance.CurrentHero.Recipes);
		}
	}
}