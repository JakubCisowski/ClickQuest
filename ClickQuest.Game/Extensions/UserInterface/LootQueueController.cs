using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using ClickQuest.Game.Core.Items;
using ClickQuest.Game.UserInterface.Windows;
using MaterialDesignThemes.Wpf;

namespace ClickQuest.Game.Extensions.UserInterface
{
	public class LootQueueEntry
	{
		public string LootName{ get; set; }
		public Rarity LootRarity { get; set; }
		public PackIconKind LootIconKind { get; set; }
		public int Quantity { get; set; }

		public LootQueueEntry(string lootName, Rarity lootRarity, PackIconKind lootIconKind, int quantity)
		{
			LootName = lootName;
			LootRarity = lootRarity;
			LootIconKind = lootIconKind;
			Quantity = quantity;
		}

		public LootQueueEntry()
		{
			
		}
	}
	
	public static class LootQueueController
	{
		public const double QueueIntervalMilliseconds = 500;

		public static List<LootQueueEntry> LootQueue { get; set; }

		private static DispatcherTimer _timer;

		static LootQueueController()
		{
			LootQueue = new List<LootQueueEntry>();

			_timer = new DispatcherTimer()
			{
				Interval = TimeSpan.FromMilliseconds(QueueIntervalMilliseconds)
			};

			_timer.Tick += Timer_Tick;
		}

		public static void AddToQueue(string lootName, Rarity lootRarity, PackIconKind lootIconKind, int quantity = 1)
		{
			LootQueue.Add(new LootQueueEntry(lootName, lootRarity, lootIconKind, quantity));

			if (!_timer.IsEnabled)
			{
				Timer_Tick(null, null);
				_timer.Start();
			}
		}

		public static void AddToQueue(List<LootQueueEntry> lootQueueEntries)
		{
			LootQueue.AddRange(lootQueueEntries);

			if (!_timer.IsEnabled)
			{
				Timer_Tick(null, null);
				_timer.Start();
			}
		}

		private static void Timer_Tick(object source, EventArgs e)
		{
			var firstEntry = LootQueue.FirstOrDefault();

			if (firstEntry is null)
			{
				_timer.Stop();
				return;
			}

			var border = FloatingTextController.CreateFloatingTextLootBorder(firstEntry.LootName, firstEntry.LootRarity, firstEntry.LootIconKind, firstEntry.Quantity);

			(Application.Current.MainWindow as GameWindow).CreateFloatingTextLoot(border);

			LootQueue.Remove(firstEntry);

			if(LootQueue.Count == 0)
			{
				_timer.Stop();
			}
			else
			{
				MergeQueue();
			}
		}

		private static void MergeQueue()
		{
			var merged = new List<LootQueueEntry>();

			foreach (var lootQueueEntry in LootQueue)
			{
				if (merged.Any(x=>x.LootName == lootQueueEntry.LootName))
				{
					merged.FirstOrDefault(x => x.LootName == lootQueueEntry.LootName).Quantity += lootQueueEntry.Quantity;
				}
				else
				{
					merged.Add(new LootQueueEntry(lootQueueEntry.LootName, lootQueueEntry.LootRarity, lootQueueEntry.LootIconKind, lootQueueEntry.Quantity));
				}
			}

			LootQueue = merged;
		}
	}
}