using System.Collections.Generic;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class IngredientPattern
	{
		public int MaterialId { get; set; }
		public int Quantity { get; set; }
	}

	public class Recipe : Item
	{
		public int ArtifactId { get; set; }
		public List<IngredientPattern> IngredientPatterns { get; set; }
	}
}