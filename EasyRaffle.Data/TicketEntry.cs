using System;

namespace EasyRaffle.Data;

public sealed class TicketEntry
{
	public string PlayerName { get; set; } = "";

	public int BaseTickets { get; set; }

	public int BonusTickets { get; set; }

	public int FreeTickets { get; set; }

	public int VipFreeTickets
	{
		get
		{
			return FreeTickets;
		}
		set
		{
			FreeTickets = value;
		}
	}

	public int TotalTickets => Math.Max(0, BaseTickets) + Math.Max(0, BonusTickets) + Math.Max(0, FreeTickets);
}
