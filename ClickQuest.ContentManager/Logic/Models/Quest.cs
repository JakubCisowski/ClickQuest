using System.Collections.Generic;
using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;
using ClickQuest.ContentManager.Logic.Models.Interfaces;

namespace ClickQuest.ContentManager.Logic.Models;

public class Quest : IIdentifiable
{
	public int Id { get; set; }
	public bool Rare { get; set; }
	public HeroClass HeroClass { get; set; }
	public string Name { get; set; }
	public int Duration { get; set; }
	public string Description { get; set; }
	public List<QuestRewardPattern> QuestRewardPatterns { get; set; }
}