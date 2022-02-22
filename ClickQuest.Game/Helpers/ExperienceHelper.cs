using System;
using System.Linq;
using ClickQuest.Game.Models;

namespace ClickQuest.Game.Helpers;

public static class ExperienceHelper
{
	// Quest experience reward = QuestExperienceRatio * YourLvlMonsterExperience * QuestDuration / TimeToKillMonster
	public const double QuestExperienceRatio = 0.6;

	// Boss experience reward = BossMaxHp * BossMaxHealthToExperienceRatio + (BossMaxHp * ThresholdReached * BossMaxHealthToExperiencePerThresholdRatio) ^ BossMaxHealthToExperiencePerThresholdExponent
	public const double BossMaxHealthToExperienceRatio = 0.1;
	public const double BossMaxHealthToExperiencePerThresholdRatio = 0.02;
	public const double BossMaxHealthToExperiencePerThresholdExponent = 1.1;

	public const double MonsterMaxHealthToExperienceRatio = 0.1;
	public const int MonsterBonusFlatExperiencePerRegion = 10;

	// Const balance values calculated in balance spreadsheet.
	private static readonly int[] TotalLevelToExperience = { 0, 220, 505, 886, 1394, 2058, 2910, 3955, 5251, 6832, 8734, 10995, 13654, 16750, 20325, 24420, 29078, 34342, 40258, 46871, 54228, 62477, 71573, 81564, 92501, 104434, 117415, 131637, 147020, 163617, 181483, 200674, 221422, 243615, 267310, 292766, 319848, 348616, 379355, 411908, 446337, 482953, 521578, 562541, 605648, 651246, 699126, 749654, 802604, 858361, 916683, 977974, 1041975, 1109109, 1179100, 1252392, 1329074, 1408842, 1492171, 1579152, 1669454, 1763582, 1861630, 1963691, 2069397, 2179295, 2293481, 2412050, 2535100, 2662728, 2794508, 2931052, 3072459, 3218828, 3370260, 3526855, 3688715, 3855942, 4028638, 4206906, 4390850, 4580574, 4776182, 4977780, 5186148, 5400729, 5621630, 5848959, 6082823, 6323330, 6571332, 6826208, 7088068, 7357799, 7634746, 7919022, 8210738, 8510831, 8818602, 9135012, 9459339 };
	private static readonly int[] MonsterHpValuesPerLvl = { 217, 246, 274, 303, 332, 361, 390, 419, 448, 478, 507, 537, 567, 597, 627, 657, 688, 719, 749, 780, 811, 842, 874, 905, 937, 968, 1001, 1033, 1065, 1097, 1130, 1163, 1195, 1228, 1261, 1295, 1328, 1362, 1395, 1429, 1463, 1497, 1532, 1566, 1601, 1636, 1671, 1706, 1741, 1777, 1812, 1848, 1884, 1920, 1956, 1993, 2030, 2066, 2103, 2140, 2177, 2214, 2253, 2290, 2328, 2366, 2404, 2443, 2481, 2520, 2559, 2598, 2637, 2677, 2716, 2756, 2796, 2836, 2877, 2917, 2958, 2999, 3040, 3081, 3123, 3164, 3205, 3248, 3290, 3333, 3375, 3417, 3461, 3503, 3546, 3590, 3633, 3677, 3721, 3765, 3809 };
	private const double TimeToKillMonsterInSeconds = 2.5;

	public static int LevelToXp(int level)
	{
		return TotalLevelToExperience[level];
	}

	public static int XpToLevel(int xp)
	{
		for (var i = 0; i <= 100; i++)
		{
			if (TotalLevelToExperience[i] > xp)
			{
				return i - 1;
			}
		}

		return 100;
	}

	public static int CalculateMonsterXpReward(Monster monster)
	{
		var experience = (int)Math.Ceiling(monster.Health * MonsterMaxHealthToExperienceRatio);
		
		// Add flat bonus based on region.
		// Math.Ceiling to account for Magic Ore possibly reducing level requirements.
		// Level 10 region -> 1 * 10 bonus xp
		// Level 15 region -> 1.5->2 * 10 bonus xp
		experience += (int)Math.Ceiling(GameAssets.Regions.FirstOrDefault(x => x.Id == monster.SpawnRegionId).LevelRequirement / 10d) * MonsterBonusFlatExperiencePerRegion;

		return experience;
	}

	public static int CalculateBossXpReward(int bossHp, int thresholdReached)
	{
		var experience = (int)Math.Ceiling(bossHp * BossMaxHealthToExperienceRatio + Math.Pow(bossHp * thresholdReached * BossMaxHealthToExperiencePerThresholdRatio, BossMaxHealthToExperiencePerThresholdExponent));

		return experience;
	}

	public static int CalculateQuestXpReward(int questDuration)
	{
		var heroLvlMonsterExperience = (int)Math.Ceiling(MonsterMaxHealthToExperienceRatio * MonsterHpValuesPerLvl[User.Instance.CurrentHero.Level]);

		var experienceReward = (int)Math.Ceiling(QuestExperienceRatio * heroLvlMonsterExperience * questDuration / TimeToKillMonsterInSeconds);

		return experienceReward;
	}

	public static void CheckIfLeveledUpAndGrantBonuses(Hero hero)
	{
		// Check if hero leveled up - if so, grant him level up bonuses and increment their lvl.
		var xpToLevel = XpToLevel(hero.Experience);
		var lvlDifference = xpToLevel - hero.Level;

		if (lvlDifference > 0)
		{
			for (var i = 0; i < lvlDifference; i++)
			{
				hero.Level++;
				hero.GrantLevelUpBonuses();
			}
		}
	}

	public static int CalculateXpToNextLvl(Hero hero)
	{
		// Calculate how much experience is needed to level up (for hero stats panel info).
		return LevelToXp(hero.Level + 1) - hero.Experience;
	}

	public static int CalculateXpProgress(Hero hero)
	{
		// Calculate progress to next level in % (for progress bar on hero stats panel).
		return (int)(100 - (double)hero.ExperienceToNextLvl / (LevelToXp(hero.Level + 1) - LevelToXp(hero.Level)) * 100);
	}
}