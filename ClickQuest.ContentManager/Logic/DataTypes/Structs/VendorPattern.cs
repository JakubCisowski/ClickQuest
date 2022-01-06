using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Models;

namespace ClickQuest.ContentManager.Logic.DataTypes.Structs;

public class VendorPattern
{
	public int Id { get; set; }
	public int VendorItemId { get; set; }
	public RewardType VendorItemType { get; set; }
	public int Value { get; set; }
}