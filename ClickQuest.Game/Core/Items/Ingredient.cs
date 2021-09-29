using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;

namespace ClickQuest.Game.Core.Items
{
	public class Ingredient
	{
		public int Id { get; set; }
		public int Quantity { get; set; }

		[JsonIgnore]
		public Material RelatedMaterial
		{
			get
			{
				return GameAssets.Materials.FirstOrDefault(x => x.Id == Id);
			}
		}
	}
}