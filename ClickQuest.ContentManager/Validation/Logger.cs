using System;
using System.IO;

namespace ClickQuest.ContentManager.Validation;

public static class Logger
{
	public static DateTime SessionStartDate = DateTime.Now;

	public static void Log(string log)
	{
		var folderPath = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString(), "Logs", "AssetsLogs");

		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}

		var fullPath = Path.Combine(folderPath, "Logs " + SessionStartDate.ToString("dd-MM-yyyy-HH-mm-ss") + ".txt");
		var isFirstLogInThisFile = !File.Exists(fullPath);

		// Log bugs in specified format.
		using var writer = new StreamWriter(fullPath, true);

		if (isFirstLogInThisFile)
		{
			writer.WriteLine($"ASSETS LOGS - {SessionStartDate.ToString("dd.MM.yyyy - HH:mm:ss")}\n");
		}

		writer.WriteLine(log);
	}
}