using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickQuest.Game.Core.Items.Types;

namespace ClickQuest.Game.Core.Items.Patterns
{
	public class QuestRewardPattern
	{
		public int QuestRewardId{ get; set; }
		public RewardType QuestRewardType { get; set; }
		public int Quantity{ get; set; }
	}
}
