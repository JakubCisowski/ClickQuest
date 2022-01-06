using System.Collections.Generic;
using System.Text.Json.Serialization;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;

namespace ClickQuest.ContentManager.Logic.Models;



public class Recipe : Item
{
	[JsonIgnore]
	public override string Description { get; set; }

	[JsonIgnore]
	public string FullName => "Recipe: " + Name;

	public int ArtifactId { get; set; }
	public List<IngredientPattern> IngredientPatterns { get; set; }
}