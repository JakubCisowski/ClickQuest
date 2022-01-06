namespace ClickQuest.ContentManager.GameData.Models;

public interface IIdentifiable
{
	int Id { get; set; }
	string Name { get; set; }
}