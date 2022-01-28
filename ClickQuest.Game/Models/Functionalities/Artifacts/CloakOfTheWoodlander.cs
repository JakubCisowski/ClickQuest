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
			else
			{
				newFrequencies.Add(freq);
			}
		}
		
		// Check sum - if after increasing frequencies, the total is still below 1.0 (100%), then there is no need to reduce any frequencies.
		if (newFrequencies.Sum() <= 1.0)
		{
			return newFrequencies;
		}
		
		// Create a dictionary with index as key, to prevent items from being reordered by the following code (the order of frequencies matters).
		var orderedListOfFrequenciesAsDictionary = new Dictionary<int, double>();

		for (int i = 0; i < newFrequencies.Count; i++)
		{
			orderedListOfFrequenciesAsDictionary.Add(i, newFrequencies[i]);
		}

		while (totalFrequenciesIncreased >= 0)
		{
			orderedListOfFrequenciesAsDictionary = orderedListOfFrequenciesAsDictionary.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

			if (totalFrequenciesIncreased >= 0.05)
			{
				totalFrequenciesIncreased -= 0.05;

				orderedListOfFrequenciesAsDictionary[0] -= 0.05;
			}
			else
			{
				orderedListOfFrequenciesAsDictionary[0] -= totalFrequenciesIncreased;
				
				totalFrequenciesIncreased = 0;
			}
		}

		newFrequencies = orderedListOfFrequenciesAsDictionary.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();

		return newFrequencies;
	}
	
	public CloakOfTheWoodlander()
	{
		Name = "Cloak of the Woodlander";
	}
}