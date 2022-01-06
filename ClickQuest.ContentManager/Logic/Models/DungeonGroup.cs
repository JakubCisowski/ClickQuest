using System.Collections.Generic;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;

namespace ClickQuest.ContentManager.Logic.Models;

public class DungeonGroup
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Color { get; set; }
	public string Description { get; set; }
	public List<Rarity> KeyRequirementRarities { get; set; }
}