using System.Linq;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Models;

namespace ClickQuest.Game.DataTypes.Structs;

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