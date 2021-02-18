namespace ClickQuest.Items
{
	public partial class Artifact : Item
	{
		public Artifact(int id, string name, Rarity rarity, string description, int value) : base(id, name, rarity, description, value)
		{
		}

		public Artifact(Item itemToCopy) : base(itemToCopy)
		{
			
		}
	}
}