using ClickQuest.ContentManager.Logic.DataTypes.Enums;
using ClickQuest.ContentManager.Logic.Models.Interfaces;

namespace ClickQuest.ContentManager.Logic.Models;

public class Item : IIdentifiable
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int Value { get; set; }
	public Rarity Rarity { get; set; }
	public virtual string Description { get; set; }
}