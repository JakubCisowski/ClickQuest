using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class VendorPattern
	{
		public int Id { get; set; }
		public int VendorItemId { get; set; }
		public RewardType VendorItemType { get; set; }
		public int Value{ get; set; }
	}
}
