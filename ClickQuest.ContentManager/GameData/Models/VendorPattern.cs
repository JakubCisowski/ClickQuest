namespace ClickQuest.ContentManager.GameData.Models
{
    public class VendorPattern
    {
        public int Id { get; set; }
        public int VendorItemId { get; set; }
        public RewardType VendorItemType { get; set; }
        public int Value { get; set; }
    }
}