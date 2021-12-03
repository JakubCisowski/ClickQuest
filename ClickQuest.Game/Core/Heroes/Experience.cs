using System;
using ClickQuest.Game.Core.Player;

namespace ClickQuest.Game.Core.Heroes
{
	public static class Experience
	{
		// Quest experience reward = QuestExperienceRatio * YourLvlMonsterExperience * QuestDuration / TimeToKillMonster
		public const double QuestExperienceRatio = 0.75;

		public const double MonsterMaxHealthToExperienceRatio = 0.1;

		// Boss experience reward = BossMaxHp * BossMaxHealthToExperienceRatio + (BossMaxHp * ThresholdReached * BossMaxHealthToExperiencePerThresholdRatio) ^ BossMaxHealthToExperiencePerThresholdExponent
		public const double BossMaxHealthToExperienceRatio = 0.2;
		public const double BossMaxHealthToExperiencePerThresholdRatio = 0.04;
		public const double BossMaxHealthToExperiencePerThresholdExponent = 1.1;

		public static int LevelToXp(int level)
		{
			// [PRERELEASE]
			var totalLevelToExperience = new int[] { 0,220,522,1005,1780,2976,4737,7163,10477,14870,20549,27736,36668,47599,60795,76538,95124,116863,142080,171113,204314,242514,285668,334171,388429,448863,515907,590748,673156,763605,862581,970583,1089135,1217808,1357139,1508873,1672440,1848417,2038781,2242811,2461120,2695931,2946355,3214786,3500218,3805221,4128665,4473301,4837869,5225308,5634222,6067742,6524332,7007319,7515020,8050966,8616146,9208648,9832316,10488185,11174098,11894227,12649649,13441456,14267150,15131355,16035207,16979855,17966464,18996213,20066050,21181317,22343236,23553042,24811984,26121327,27482350,28896345,30364618,31888491,33469298,35108389,36807127,38566888,40394980,42287021,44244430,46268641,48361102,50523275,52763341,55076219,57463412,59933557,62481206,65107904,67815211,70612385,73493472,76468048,79529899 };

			return totalLevelToExperience[level];
		}

		public static int XpToLevel(int xp)
		{
			// [PRERELEASE]
			var totalLevelToExperience = new int[] { 0,220,522,1005,1780,2976,4737,7163,10477,14870,20549,27736,36668,47599,60795,76538,95124,116863,142080,171113,204314,242514,285668,334171,388429,448863,515907,590748,673156,763605,862581,970583,1089135,1217808,1357139,1508873,1672440,1848417,2038781,2242811,2461120,2695931,2946355,3214786,3500218,3805221,4128665,4473301,4837869,5225308,5634222,6067742,6524332,7007319,7515020,8050966,8616146,9208648,9832316,10488185,11174098,11894227,12649649,13441456,14267150,15131355,16035207,16979855,17966464,18996213,20066050,21181317,22343236,23553042,24811984,26121327,27482350,28896345,30364618,31888491,33469298,35108389,36807127,38566888,40394980,42287021,44244430,46268641,48361102,50523275,52763341,55076219,57463412,59933557,62481206,65107904,67815211,70612385,73493472,76468048,79529899 };

			for (int i = 0; i <= 100; i++)
			{
				if(totalLevelToExperience[i] > xp)
				{
					return i - 1;
				}
			}

			return 100;
		}

		public static int CalculateMonsterXpReward(int monsterHp)
		{
			var experience = (int) Math.Ceiling(monsterHp * MonsterMaxHealthToExperienceRatio);

			return experience;
		}
		
		public static int CalculateBossXpReward(int bossHp, int thresholdReached)
		{
			var experience = (int) Math.Ceiling(bossHp * BossMaxHealthToExperienceRatio + Math.Pow((bossHp * thresholdReached * BossMaxHealthToExperiencePerThresholdRatio), BossMaxHealthToExperiencePerThresholdExponent));

			return experience;
		}
		
		public static int CalculateQuestXpReward(int questDuration)
		{
			double timeToKillMonster = 2.5;

			// [PRERELEASE]
			var monsterHpValuesPerLvl = new int[]{ 217,246,274,303,332,361,390,419,448,478,507,537,567,597,627,657,688,719,749,780,811,842,874,905,937,968,1001,1033,1065,1097,1130,1163,1195,1228,1261,1295,1328,1362,1395,1429,1463,1497,1532,1566,1601,1636,1671,1706,1741,1777,1812,1848,1884,1920,1956,1993,2030,2066,2103,2140,2177,2214,2253,2290,2328,2366,2404,2443,2481,2520,2559,2598,2637,2677,2716,2756,2796,2836,2877,2917,2958,2999,3040,3081,3123,3164,3205,3248,3290,3333,3375,3417,3461,3503,3546,3590,3633,3677,3721,3765,3809 };

			int yourLvlMonsterExperience = (int) Math.Ceiling(MonsterMaxHealthToExperienceRatio * monsterHpValuesPerLvl[User.Instance.CurrentHero.Level]);


			var experience = (int) Math.Ceiling((QuestExperienceRatio * yourLvlMonsterExperience * questDuration) / timeToKillMonster);

			return experience;
		}

		public static void CheckIfLeveledUpAndGrantBonuses(Hero hero)
		{
			// Check if hero leveled up - if so, grant him level up bonuses and increment his lvl.
			int xpToLevel = XpToLevel(hero.Experience);
			int lvlDifference = xpToLevel - hero.Level;

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
			// Calculate how many experience is needed to level up (for hero stats panel info).
			return LevelToXp(hero.Level + 1) - hero.Experience;
		}

		public static int CalculateXpProgress(Hero hero)
		{
			// Calculate progress to next level in % (for progress bar on hero stats panel).
			return (int) (100 - (double) hero.ExperienceToNextLvl / (LevelToXp(hero.Level + 1) - LevelToXp(hero.Level)) * 100);
		}
	}
}