using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickQuest.Game.Core.Items
{
	public class QuestRewardPattern
	{
		public int Id{ get; set; }
		public RewardType RewardType{ get; set; }
		public int Quantity{ get; set; }
	}
}
