using System;
using System.Collections.Generic;
using EasyRaffle.Data;

namespace EasyRaffle;

public sealed class SessionState
{
	public DateTimeOffset SavedAt { get; set; } = DateTimeOffset.UtcNow;

	public bool ClosedCleanly { get; set; }

	public int ActiveStartMil { get; set; }

	public int BonusTicketsRemaining { get; set; }

	public List<TicketEntry> Entries { get; set; } = new List<TicketEntry>();

	public Dictionary<string, int> VipUsedFreebies { get; set; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

	public string VenueMacro { get; set; } = string.Empty;

	public bool EnableTicketRangeTells { get; set; }

	public string TicketRangeTemplate { get; set; } = "/tell {PlayerName} Your tickets are %ticketrange";

	public string LastTellTarget { get; set; } = string.Empty;

	public string DiscordWebhookUrl { get; set; } = string.Empty;
}
