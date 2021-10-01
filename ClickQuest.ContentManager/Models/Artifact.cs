namespace ClickQuest.ContentManager.Models
{
	public class Artifact
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Rarity Rarity { get; set; }
		public string Description { get; set; }
		public int Value { get; set; }

		public Artifact()
		{
			
		}
	}
}