using System.Collections.Generic;
using System.Linq;
using ClickQuest.Game.DataTypes.Structs;

namespace ClickQuest.Game.Models.Functionalities.Artifacts;

// While in Woodland Cascades, you have an increased chance to find rare Items. The chance to loot Items that have a drop rate of 5% or less is doubled,
// while the chance to loot the most common Items is reduced appropriately.
public class CloakOfTheWoodlander : ArtifactFunctionality
{
	// Special case function - to be called from the 'GrantVictoryBonuses' function in Monster.cs, if player is currently in Woodland Cascades.
	public static List<double> ModifyWoodlandCascadesLootFrequencies(List<double> oldFrequencies)
	{
		var newFrequencies = new List<double>();
		double totalFrequenciesIncreased = 0;

		foreach (var freq in oldFrequencies)
		{
			if (freq <= 0.05)
			{
				totalFrequenciesIncreased += freq;
				newFrequencies.Add(freq * 2);
			}
		}
		
		while (totalFrequenciesIncreased >= 0)
		{
			newFrequencies = newFrequencies.OrderByDescending(x=>x).ToList();

			if (totalFrequenciesIncreased >= 0.05)
			{
				totalFrequenciesIncreased -= 0.05;

				newFrequencies[0] -= 0.05;
			}
			else
			{
				newFrequencies[0] -= totalFrequenciesIncreased;
				
				totalFrequenciesIncreased = 0;
			}
		}

		return newFrequencies;
	}
	
	public CloakOfTheWoodlander()
	{
		Name = "Cloak of the Woodlander";
	}
}