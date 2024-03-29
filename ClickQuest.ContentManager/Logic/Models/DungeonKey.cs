using ClickQuest.ContentManager.Logic.DataTypes.Enums;

namespace ClickQuest.ContentManager.Logic.Models;

public class DungeonKey
{
	public int Id { get; set; }
	public string Name { get; set; }
	public Rarity Rarity { get; set; }
	public string Description { get; set; }
	public int Value { get; set; }
}