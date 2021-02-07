using System;

namespace ClickQuest.Heroes
{
    public static class Experience
    {
        public static int Equate(double xp)
        {
            return (int)Math.Floor(xp + (300 * Math.Pow(2, xp / 7)));
        }

        public static int LevelToXP(int level)
        {
            level += 1;
            double xp = 0;

            for (int i = 1; i < level; i++)
            {
                xp += Equate(i);
            }

            return (int)Math.Floor(xp / 16);
        }

        public static int XPToLevel(int xp)
        {
            int level = 1;

            while (LevelToXP(level) <= xp)
            {
                level++;
            }

            return level - 1;
        }

        public static int CalculateMonsterXpReward(int monsterHp)
        {
            return (int)Math.Ceiling(monsterHp / 10d);
        }

        public static void CheckIfLeveledUp(Hero hero)
        {
            // Check if hero leveled up - if so, grant him level up bonuses and increment his lvl.
            int xpToLevel = XPToLevel(hero.Experience);
            int lvlDifference = xpToLevel - hero.Level;

            if (lvlDifference > 0)
            {
                for (int i = 0; i < lvlDifference; i++)
                {
                    hero.Level++;
                    hero.GrantLevelUpBonuses();
                }
            }
        }

        public static int CalculateXpToNextLvl(Hero hero)
        {
            // Calculate how many experience is needed to level up (for hero stats panel info).
            return LevelToXP(hero.Level + 1) - hero.Experience;
        }

        public static int CalculateXpProgress(Hero hero)
        {
            // Calculate progress to next level in % (for progress bar on hero stats panel).
            return (int)(100 - (((double)hero.ExperienceToNextLvl / (LevelToXP(hero.Level+1) - LevelToXP(hero.Level))) * 100));
        }
    }
}