using System;
using ClickQuest.Game.Core.GameData;

namespace ClickQuest.Game.Core.Items.Artifacts
{
	// Reduce the Level Requirement of all Regions by 5.
	public class MagicOre : ArtifactFunctionality
	{
		private const int RegionLevelRequirementReduction = 5;
		
		public override void OnEquip()
		{
			foreach (var region in GameAssets.Regions)
			{
				// To prevent Level Requirement from falling below 0.
				region.LevelRequirement = Math.Max(0, region.LevelRequirement - 5);
			}
		}

		public override void OnUnequip()
		{
			foreach (var region in GameAssets.Regions)
			{
				region.LevelRequirement += 5;
			}
		}

		public MagicOre()
		{
			Name = "Magic Ore";
		}
	}
}