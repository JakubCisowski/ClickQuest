﻿using System.Collections.Generic;
using ClickQuest.ContentManager.Logic.DataTypes.Structs;

namespace ClickQuest.ContentManager.Logic.Models;

public static class GameAssets
{
	public static List<Artifact> Artifacts { get; set; }
	public static List<Blessing> Blessings { get; set; }
	public static List<Boss> Bosses { get; set; }
	public static List<DungeonGroup> DungeonGroups { get; set; }
	public static List<DungeonKey> DungeonKeys { get; set; }
	public static List<Ingot> Ingots { get; set; }
	public static List<Dungeon> Dungeons { get; set; }
	public static List<Material> Materials { get; set; }
	public static List<Monster> Monsters { get; set; }
	public static List<Quest> Quests { get; set; }
	public static List<Recipe> Recipes { get; set; }
	public static List<Region> Regions { get; set; }
	public static List<VendorPattern> PriestOffer { get; set; }
	public static List<VendorPattern> ShopOffer { get; set; }
	public static List<GameMechanicsTab> GameMechanicsTabs { get; set; }
}