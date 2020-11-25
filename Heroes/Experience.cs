using System;
using System.IO;
using System.Diagnostics;

namespace ClickQuest.Heroes
{
    public static class Experience
    {
        public static int Equate(double xp)
        {
            return (int) Math.Floor(xp + 300 * Math.Pow(2, xp / 7));
        }
    
        public static int LevelToXP(int level)
        {
            double xp = 0;
    
            for (int i = 1; i < level; i++)
            {
                xp += Equate(i);
            }
    
            return (int) Math.Floor(xp / 32);
        }

        public static int XPToLevel(int xp)
        {
            int level = 1;

            while (LevelToXP(level) < xp)
            {
                level++;
            }
    
            return level;
        }
    }
}