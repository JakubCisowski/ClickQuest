using System.Linq;
using ClickQuest.Data;

namespace ClickQuest.Items
{
	public class Ingredient
	{
		public int Id { get; set; }
		public int Quantity { get; set; }

		public Material RelatedMaterial
		{
			get
			{
				return GameData.Materials.FirstOrDefault(x => x.Id == Id);
			}
		}
	}
}