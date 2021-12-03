using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ClickQuest.ContentManager.GameData.Models
{
    public class IngredientPattern
    {
        public int MaterialId { get; set; }
        public int Quantity { get; set; }
    }

    public class Recipe : Item
    {
        [JsonIgnore]
        public override string Description { get; set; }

        [JsonIgnore]
        public string FullName => "Recipe: " + Name;

        public int ArtifactId { get; set; }
        public List<IngredientPattern> IngredientPatterns { get; set; }
    }
}