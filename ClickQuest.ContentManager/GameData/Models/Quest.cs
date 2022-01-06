using System.Collections.Generic;

namespace ClickQuest.ContentManager.GameData.Models;

public enum HeroClass { All, Slayer, Venom }

public class QuestRewardPattern
{
	public int QuestRewardId { get; set; }
	public RewardType QuestRewardType { get; set; }
	public int Quantity { get; set; }
}

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