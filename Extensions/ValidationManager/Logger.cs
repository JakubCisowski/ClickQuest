using System;
using System.IO;

namespace ClickQuest.Extensions.ValidationManager
{
    public static class Logger
    {
        public static DateTime SessionStartDate = DateTime.Now;

        public static void Log(string log)
        {
            // Log bugs in specified format.
            using var writer = new StreamWriter(Path.Combine("C:/Programowanie/Fun/ClickQuest", "Logs", "Logs " + SessionStartDate.ToString("dd-MM-yyyy-HH-mm-ss") + ".txt"), true);

            writer.WriteLine(log);
        }
    }
}