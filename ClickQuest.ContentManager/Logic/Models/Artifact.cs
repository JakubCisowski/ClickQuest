using ClickQuest.ContentManager.Logic.DataTypes.Enums;

namespace ClickQuest.ContentManager.Logic.Models;

public class Artifact : Item
{
	public string Lore { get; set; }
	public string ExtraInfo { get; set; }
	public ArtifactType ArtifactType { get; set; }
}