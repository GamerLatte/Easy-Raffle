using System.Collections.Generic;
using System.Linq;

namespace Raffler.Windows;

public class VIPConfigList
{
	public List<VIPConfig> VIPConfigs { get; set; } = new List<VIPConfig>();

	public void AddVIP(string name, int tickets, uint color, bool allowExtraTickets, bool allowBogoOverride)
	{
		VIPConfig vIPConfig = VIPConfigs.FirstOrDefault((VIPConfig v) => v.VIPName == name);
		if (vIPConfig != null)
		{
			vIPConfig.FreeTickets = tickets;
			vIPConfig.VIPColor = color;
			vIPConfig.AllowExtraTickets = allowExtraTickets;
			vIPConfig.AllowBogoOverride = allowBogoOverride;
		}
		else
		{
			VIPConfigs.Add(new VIPConfig
			{
				VIPName = name,
				FreeTickets = tickets,
				VIPColor = color,
				AllowExtraTickets = allowExtraTickets,
				AllowBogoOverride = allowBogoOverride
			});
		}
	}

	public void RemoveVIP(string name)
	{
		VIPConfigs.RemoveAll((VIPConfig v) => v.VIPName == name);
	}
}
