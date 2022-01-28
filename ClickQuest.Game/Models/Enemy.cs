using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.DataTypes.Enums;
using ClickQuest.Game.Helpers;
using ClickQuest.Game.Models.Interfaces;

namespace ClickQuest.Game.Models;

public abstract class Enemy : INotifyPropertyChanged, IIdentifiable
{
	public event PropertyChangedEventHandler PropertyChanged;

	protected int _currentHealth;

	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int Health { get; set; }
	public abstract int CurrentHealth { get; set; }

	[JsonIgnore]
	public int CurrentHealthProgress { get; set; }

	public string Image { get; set; }

	public Enemy()
	{
		CurrentHealthProgress = 100;
	}

	public abstract Enemy CopyEnemy();

	protected int CalculateCurrentHealthProgress()
	{
		// Calculate killing progress in % (for progress bar on monster button).
		return (int)((double)CurrentHealth / Health * 100);
	}

	public abstract void HandleEnemyDeathIfDefeated();

	public abstract void GrantVictoryBonuses();

	protected void CheckForDungeonKeyDrop()
	{
		var dungeonKeyRarityChances = DungeonKey.CreateRarityChancesList(Health);

		var position = CollectionsHelper.RandomizeFrequenciesListPosition(dungeonKeyRarityChances);

		// Grant dungeon key after if algorithm didn't roll empty loot.
		if (position != 0)
		{
			var dungeonKey = User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity)(position - 1));
			dungeonKey.AddItem();
		}
	}
}