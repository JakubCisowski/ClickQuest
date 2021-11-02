using System.Collections.Generic;

namespace ClickQuest.ContentManager.GameData.Models
{
	public class Dungeon : IIdentifiable
	{
		public int Id { get; set; }
		public int DungeonGroupId { get; set; }
		public string Name { get; set; }
		public string Background { get; set; }
		public string Description { get; set; }
		public List<int> BossIds { get; set; }
	}
}