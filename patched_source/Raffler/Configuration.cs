using System;
using System.Collections.Generic;
using Dalamud.Configuration;

namespace Raffler;

[Serializable]
public class Configuration : IPluginConfiguration
{
	public class VipRule
	{
		public int FirstNFreeTickets { get; set; } = 15;
	}

	public int VipFreeBogoPairsPerPurchase;

	public int VipBogoPairsGiven;

	public BogoType RaffleBogoType;

	public int BogoBonusTickets = 1;

	public float TicketCost = 500f;

	public bool CountBonusTicketsTowardPot { get; set; }

	public bool CountVipTicketsTowardPot { get; set; }

	public string DiscordWebhookUrl { get; set; } = "";

	public bool ShowWorldNameInTicket { get; set; }

	public int? ActiveSessionStartingPotMillions { get; set; }

	public string VenueMacro { get; set; } = "/yell We're running a 50/50 Raffle! The pot is at $pot , there are $bonusticketsleft buy one get one tickets left! Tickets cost 100K each!";

	public bool ShowDebugOption { get; set; } = true;

	public bool EnableTicketRangeTells { get; set; }

	public int StartingPotMillions { get; set; } = 10;

	public int BogoSessionLimit { get; set; }

	public bool UseFiftyFiftySplit { get; set; } = true;

	public bool DefaultApplyBogoForEntry { get; set; } = true;

	public string TicketRangeTemplate { get; set; } = "/tell {PlayerName} Your tickets are %ticketrange";

	public int StartingGil { get; set; }

	public int Version { get; set; }

	public List<string> LastDiscordChunkMessageIds { get; set; } = new List<string>();

	public bool IsConfigWindowMovable { get; set; } = true;

	public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

	public Dictionary<string, VipRule> VipRules { get; set; } = new Dictionary<string, VipRule>();

	public void Save()
	{
		Plugin.PluginInterface.SavePluginConfig((IPluginConfiguration)(object)this);
	}
}
