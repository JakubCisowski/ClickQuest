using System;

namespace ClickQuest.Game.Core.Heroes
{
    public static class Experience
    {
        public static int Equate(double xp)
        {
            return (int)Math.Floor(xp + 300 * Math.Pow(2, xp / 7));
        }

        public static int LevelToXp(int level)
        {
            level += 1;
            double xp = 0;

            for (var i = 1; i < level; i++)
            {
                xp += Equate(i);
            }

            return (int)Math.Floor(xp / 16);
        }

        public static int XpToLevel(int xp)
        {
            var level = 1;

            while (LevelToXp(level) <= xp)
            {
                level++;
            }

            return level - 1;
        }

        public static int CalculateMonsterXpReward(int monsterHp)
        {
            var experience = (int)Math.Ceiling(monsterHp / 10d);

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
            return (int)(100 - (double)hero.ExperienceToNextLvl / (LevelToXp(hero.Level + 1) - LevelToXp(hero.Level)) * 100);
        }
    }
}