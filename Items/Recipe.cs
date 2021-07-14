using ClickQuest.Data;
using ClickQuest.Player;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ClickQuest.Items
{
	public partial class Recipe : Item

	{
		private int _artifactId;
		private Dictionary<int, int> _materialIds;

		public int ArtifactId
		{
			get
			{ return _artifactId; }
			set { _artifactId = value; }
		}

		public Artifact Artifact
		{
			get
			{
				return GameData.Artifacts.FirstOrDefault(x => x.Id == ArtifactId);
			}
		}

		// Key is the Id of the material; value is the amount needed.
		[NotMapped]
		public Dictionary<int, int> MaterialIds
		{
			get { return _materialIds; }
			set { _materialIds = value; }
		}

		[NotMapped]
		public string RequirementsDescription { get; private set; }

		[NotMapped]
		public int IngotsRequired
		{
			get
			{
				return (int)(Rarity + 1) * 1000;
			}
		}

		public void UpdateRequirementsDescription()
		{
			MaterialIds = GameData.Recipes.FirstOrDefault(x => x.Id == this.Id)?.MaterialIds;

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

		public Recipe() : base ()
		{

		}

		public override Recipe CopyItem()
		{
			Recipe copy = new Recipe();

			copy.Id = Id;
			copy.Name = Name;
			copy.Rarity = Rarity;
			copy.Value = Value;
			copy.Description = Description;
			copy.Quantity = 1;
			copy.ArtifactId = ArtifactId;
			copy.MaterialIds = MaterialIds;
			copy.RequirementsDescription = RequirementsDescription;

			return copy;
		}

		public override void AddAchievementProgress(int amount)
		{
			User.Instance.Achievements.IncreaseAchievementValue(NumericAchievementType.RecipesGained, amount);
		}
	}
}