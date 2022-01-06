using ClickQuest.ContentManager.Logic.DataTypes.Enums;

namespace ClickQuest.ContentManager.Logic.DataTypes.Structs;

public class QuestRewardPattern
{
	public int QuestRewardId { get; set; }
	public RewardType QuestRewardType { get; set; }
	public int Quantity { get; set; }
}