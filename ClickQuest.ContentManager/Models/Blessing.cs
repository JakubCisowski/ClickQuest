namespace ClickQuest.ContentManager.Models
{
	public class Blessing
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public BlessingType Type { get; set; }
		public int Rarity { get; set; }
		public int Duration { get; set; }
		public string Description { get; set; }
		public int Buff { get; set; }
		public int Value { get; set; }

		public Blessing()
		{
			
		}
	}
}