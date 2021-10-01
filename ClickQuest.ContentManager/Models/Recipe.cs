using System.Collections.Generic;

namespace ClickQuest.ContentManager.Models
{
	public class Ingredient
	{
		public int Id { get; set; }
		public int Quantity { get; set; }
	}
	
	public class Recipe
	{
		public int Id { get; set; }
		public int ArtifactId { get; set; }
		public List<Ingredient> Ingredients { get; set; }
		public string Name { get; set; }
		public Rarity Rarity { get; set; }
		public int Value { get; set; }

		public Recipe()
		{
			
		}
	}
}