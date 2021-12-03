using ClickQuest.Game.Core.Items.Types;

namespace ClickQuest.Game.Core.Items.Patterns
{
    public class QuestRewardPattern
    {
        public int QuestRewardId { get; set; }
        public RewardType QuestRewardType { get; set; }
        public int Quantity { get; set; }
    }
}