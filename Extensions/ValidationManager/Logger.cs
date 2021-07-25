using System;
using System.Collections.Generic;
using System.IO;

namespace ClickQuest.Extensions.ValidationManager
{
	public static class Logger
	{
		public static void Log(List<string> logs)
		{
			// Log bugs in specified format.
			using var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "Logs", "Logs " + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".txt"));

			foreach (string log in logs)
			{
				writer.WriteLine(log);
			}
		}
	}
}