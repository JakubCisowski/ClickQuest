namespace ClickQuest.ContentManager.GameData.Models
{
	public class Blessing : IIdentifiable
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public BlessingType Type { get; set; }
		public Rarity Rarity { get; set; }
		public int Duration { get; set; }
		public string Description { get; set; }
		public string Lore { get; set; }
		public int Buff { get; set; }
		public int Value { get; set; }

		public Blessing()
		{
			
		}
	}
}