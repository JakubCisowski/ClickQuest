using System.Collections.Generic;
using System.Linq;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;
using ClickQuest.ContentManager.Logic.Models.Interfaces;

namespace ClickQuest.ContentManager.Logic.Models;

public class Monster : IIdentifiable
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int Health { get; set; }
	public string Image { get; set; }
	public List<MonsterLootPattern> MonsterLootPatterns { get; set; }
}