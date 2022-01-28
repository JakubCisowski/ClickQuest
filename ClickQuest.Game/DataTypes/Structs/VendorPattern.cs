using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.DataTypes.Structs;

public class VendorPattern
{
	public int Id { get; set; }
	public int VendorItemId { get; set; }
	public RewardType VendorItemType { get; set; }
}