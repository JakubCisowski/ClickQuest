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

		[NotMapped]
		public Dictionary<int, int> MaterialIds
		{
			get { return _materialIds; }
			set { _materialIds = value; }
		}

		[NotMapped]
		public string RequirementsDescription { get; private set; }

		public Recipe(int id, string name, Rarity rarity, string description, int value, int artifactId) : base(id, name, rarity, description, value)
		{
			ArtifactId = artifactId;
			MaterialIds = new Dictionary<int, int>();

			//UpdateRequirementsDescription();
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
	}
}