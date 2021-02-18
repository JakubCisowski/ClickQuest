using ClickQuest.Data;
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

		public Recipe(int id, string name, Rarity rarity, string description, int value, int artifactId) : base(id, name, rarity, description, value)
		{
			ArtifactId = artifactId;
			MaterialIds = new Dictionary<int, int>();
		}

		public void UpdateRequirementsDescription()
		{
			RequirementsDescription = "Materials required: ";

			foreach (var elem in MaterialIds)
			{
				// Add name
				RequirementsDescription += Database.Materials.FirstOrDefault(x => x.Id == elem.Key).Name;
				// Add quantity
				RequirementsDescription = RequirementsDescription + ": " + elem.Value + "; ";
			}
		}

		public Recipe(Item itemToCopy) : base(itemToCopy)
		{
			if (itemToCopy is Recipe recipe)
			{
				ArtifactId = recipe.ArtifactId;
				MaterialIds = recipe.MaterialIds;
				RequirementsDescription = recipe.RequirementsDescription;
			}
		}
	}
}