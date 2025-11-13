using System;
using System.Collections.Generic;

namespace Raffler;

public sealed class VipRuntime
{
	private readonly Dictionary<string, int> usedFreebies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

	public int GetUsed(string name)
	{
		if (!usedFreebies.TryGetValue(name, out var value))
		{
			return 0;
		}
		return value;
	}

	public void AddUsed(string name, int add)
	{
		if (add > 0)
		{
			usedFreebies[name] = GetUsed(name) + add;
		}
	}

	public void Reset()
	{
		usedFreebies.Clear();
	}

	public IReadOnlyDictionary<string, int> Snapshot()
	{
		return new Dictionary<string, int>(usedFreebies, StringComparer.OrdinalIgnoreCase);
	}

	public void LoadFrom(Dictionary<string, int>? source)
	{
		usedFreebies.Clear();
		if (source == null)
		{
			return;
		}
		foreach (KeyValuePair<string, int> item in source)
		{
			usedFreebies[item.Key] = item.Value;
		}
	}
}
