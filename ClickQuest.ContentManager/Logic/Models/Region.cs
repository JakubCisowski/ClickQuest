using System.Collections.Generic;
using System.Linq;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;
using ClickQuest.ContentManager.Logic.Models.Interfaces;

namespace ClickQuest.ContentManager.Logic.Models;



public class Region : IIdentifiable
{
	public int Id { get; set; }
	public int LevelRequirement { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Background { get; set; }
	public List<MonsterSpawnPattern> MonsterSpawnPatterns { get; set; }
}