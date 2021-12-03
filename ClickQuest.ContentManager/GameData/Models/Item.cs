namespace ClickQuest.ContentManager.GameData.Models
{
    public class Item : IIdentifiable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        public Rarity Rarity { get; set; }
        public virtual string Description { get; set; }
    }
}