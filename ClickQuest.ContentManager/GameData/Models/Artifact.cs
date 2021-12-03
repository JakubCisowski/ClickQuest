namespace ClickQuest.ContentManager.GameData.Models
{
    public class Artifact : Item
    {
        public string Lore { get; set; }
        public string ExtraInfo { get; set; }
        public ArtifactType ArtifactType { get; set; }
    }
}