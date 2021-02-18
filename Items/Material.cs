namespace ClickQuest.Items
{
	public partial class Material : Item
	{
		public Material(int id, string name, Rarity rarity, string description, int value) : base(id, name, rarity, description, value)
		{
		}

		public Material(Item itemToCopy) : base(itemToCopy)
		{

		}
	}
}