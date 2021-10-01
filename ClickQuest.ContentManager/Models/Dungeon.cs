using System.Collections.Generic;

namespace ClickQuest.ContentManager.Models
{
	public class Dungeon
	{
		public int Id { get; set; }
		public int DungeonGroupId { get; set; }
		public string Name { get; set; }
		public string Background { get; set; }
		public string Description { get; set; }
		public List<int> BossIds { get; set; }

		public Dungeon()
		{
			
		}
	}
}