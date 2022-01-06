using System.Collections.Generic;
using System.ComponentModel;
using ClickQuest.Game.DataTypes.Structs;
using ClickQuest.Game.Models.Interfaces;

namespace ClickQuest.Game.Models;

public class Region : INotifyPropertyChanged, IIdentifiable
{
	public event PropertyChangedEventHandler PropertyChanged;

	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Background { get; set; }
	public List<MonsterSpawnPattern> MonsterSpawnPatterns { get; set; }
	public int LevelRequirement { get; set; }
}