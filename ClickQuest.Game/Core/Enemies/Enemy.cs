using System.ComponentModel;
using System.Linq;
using System.Text.Json.Serialization;
using ClickQuest.Game.Core.GameData;
using ClickQuest.Game.Core.Interfaces;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.Core.Player;
using ClickQuest.Game.Extensions.Collections;
using ClickQuest.Game.UserInterface.Pages;

namespace ClickQuest.Game.Core.Enemies
{
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
			return (int) ((double) CurrentHealth / Health * 100);
		}

		public abstract void HandleEnemyDeathIfDefeated();

		public abstract void GrantVictoryBonuses();

		protected void CheckForDungeonKeyDrop()
		{
			var DungeonKeyRarityChances = DungeonKey.CreateRarityChancesList(Health);

			int position = CollectionsController.RandomizeFreqenciesListPosition(DungeonKeyRarityChances);

			// Grant dungeon key after if algorithm didn't roll empty loot.
			if (position != 0)
			{
				var dungeonKey = User.Instance.DungeonKeys.FirstOrDefault(x => x.Rarity == (Rarity) (position - 1));
				dungeonKey.AddItem();

				// [PRERELEASE] Display dungeon key drop.
				// (GameAssets.CurrentPage as RegionPage).TestRewardsBlock.Text += $". You've got a {(Rarity) (position - 1)} Dungeon Key!";
			}
		}
	}
}