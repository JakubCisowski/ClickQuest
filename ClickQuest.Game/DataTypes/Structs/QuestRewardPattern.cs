using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.DataTypes.Structs;

public class QuestRewardPattern
{
	public int QuestRewardId { get; set; }
	public RewardType QuestRewardType { get; set; }
	public int Quantity { get; set; }
}