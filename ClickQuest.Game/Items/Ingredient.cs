using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Data.GameData;

namespace ClickQuest.Game.Items
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
				return GameData.Materials.FirstOrDefault(x => x.Id == Id);
			}
		}
	}
}