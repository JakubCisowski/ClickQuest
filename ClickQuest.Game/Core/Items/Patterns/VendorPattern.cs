using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickQuest.Game.Core.Items.Types;

namespace ClickQuest.Game.Core.Items.Patterns
{
	public class VendorPattern
	{
		public int Id { get; set; }
		public int VendorItemId { get; set; }
		public RewardType VendorItemType { get; set; }
		public int Value{ get; set; }
	}
}
