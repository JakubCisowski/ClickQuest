using System.Collections.Generic;
using ClickQuest.Game.DataTypes.Enums;

namespace ClickQuest.Game.Models;

public class DungeonGroup
{
	public int Id { get; set; }
	public string Name { get; set; }
	public List<Rarity> KeyRequirementRarities { get; set; }
	public string Color { get; set; }
}