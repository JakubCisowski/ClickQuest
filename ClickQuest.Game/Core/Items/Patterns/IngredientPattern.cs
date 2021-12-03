using System.Linq;
using ClickQuest.Game.Core.GameData;

namespace ClickQuest.Game.Core.Items.Patterns
{
	public class IngredientPattern
	{
		public int MaterialId { get; set; }
		public int Quantity { get; set; }

		public Material RelatedMaterial
		{
			get
			{
				return GameAssets.Materials.FirstOrDefault(x => x.Id == MaterialId);
			}
		}
	}
}