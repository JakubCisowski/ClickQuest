using System.IO;
using System.Collections.Generic;
using System;

namespace ClickQuest.Data
{
    public static partial class Logger
    {
        public static void Log(List<string> logs)
        {
            using var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "Logs", "Logs " + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")));

            foreach (var log in logs)
            {
                writer.WriteLine(log);
            }
        }
    }
}