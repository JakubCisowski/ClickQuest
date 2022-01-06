using System.Collections.Generic;
using ClickQuest.ContentManager.Logic.Models.Interfaces;

namespace ClickQuest.ContentManager.Logic.Models;

public class Dungeon : IIdentifiable
{
	public int Id { get; set; }
	public int DungeonGroupId { get; set; }
	public string Name { get; set; }
	public string Background { get; set; }
	public string Description { get; set; }
	public List<int> BossIds { get; set; }
}