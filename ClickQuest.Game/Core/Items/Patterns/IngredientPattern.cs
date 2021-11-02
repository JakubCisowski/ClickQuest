using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;

namespace ClickQuest.Game.Core.Items.Patterns
{
	public class IngredientPattern
	{
		public int MaterialId { get; set; }
		public int Quantity { get; set; }

		[JsonIgnore]
		public Material RelatedMaterial
		{
			get
			{
				return GameAssets.Materials.FirstOrDefault(x => x.Id == MaterialId);
			}
		}
	}
}