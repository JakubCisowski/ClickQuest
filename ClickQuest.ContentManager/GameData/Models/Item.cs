using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class Item : IIdentifiable
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Value { get; set; }
		public Rarity Rarity { get; set; }
		public string Description { get; set; }

		public Item()
		{

		}
	}
}
