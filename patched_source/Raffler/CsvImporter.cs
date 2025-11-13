using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Raffler.Data;

namespace Raffler;

public class CsvImporter
{
	private readonly Plugin plugin;

	public CsvImporter(Plugin plugin)
	{
		this.plugin = plugin;
	}

	public void LoadCsvFromFile(string filePath)
	{
		if (!File.Exists(filePath))
		{
			Plugin.Log.Error("File not found: " + filePath, Array.Empty<object>());
			Plugin.ChatGui.PrintError("❌ File not found.", (string)null, (ushort?)null);
			return;
		}
		string[] array = File.ReadAllLines(filePath);
		if (array.Length == 0)
		{
			Plugin.ChatGui.PrintError("❌ File is empty.", (string)null, (ushort?)null);
			return;
		}
		if (array[0].StartsWith("TicketNumber", StringComparison.OrdinalIgnoreCase))
		{
			array = array.Skip(1).ToArray();
		}
		Dictionary<string, (int, int)> dictionary = new Dictionary<string, (int, int)>();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split(',');
			if (array3.Length < 2)
			{
				continue;
			}
			string text = array3[1].Trim();
			if (!string.IsNullOrWhiteSpace(text))
			{
				if (!dictionary.ContainsKey(text))
				{
					dictionary[text] = (0, 0);
				}
				dictionary[text] = (dictionary[text].Item1 + 1, dictionary[text].Item2);
			}
		}
		plugin.Entries.Clear();
		foreach (KeyValuePair<string, (int, int)> item in dictionary)
		{
			plugin.Entries.Add(new TicketEntry
			{
				PlayerName = item.Key,
				BaseTickets = item.Value.Item1,
				BonusTickets = item.Value.Item2
			});
		}
		plugin.SaveEntries();
		Plugin.ChatGui.Print($"✅ Loaded {dictionary.Count} unique players from file.", (string)null, (ushort?)null);
	}
}
