using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using ECommons.Automation;
using Lumina.Excel.Sheets;
using Raffler.Data;
using Raffler.UI;

namespace Raffler.Windows;

public class MainWindow : Window, IDisposable
{
	private sealed class SessionSnapshot
	{
		public int BaseTickets { get; init; }

		public int PaidBonusTickets { get; init; }

		public int VipFreeTickets { get; init; }

		public int CountedTickets { get; init; }

		public int IssuedTickets { get; init; }

		public float TicketCostGil { get; init; }

		public float GilCounted { get; init; }

		public int BonusTicketsLeft { get; init; }

		public int StartMil { get; init; }

		public float StartingPotGil { get; init; }

		public float FinalPotGil { get; init; }

		public bool CountBonusTowardPot { get; init; }
	}

	private bool showImportSettingsPopup;

	private string pendingCsvPath = "";

	private int importStartingPot = 10;

	private BogoType importBogoType;

	private string playernameandworld = "";

	private bool showFileTools;

	private bool showDiscordTools;

	private bool showRepeatBuyerWarning;

	private bool allowRepeatBuyer;

	private string lastBuyerName = "";

	private bool showResetConfirm;

	private bool pluginExpired;

	private readonly FilePicker filePicker = new FilePicker();

	private string rafflerImg;

	private Plugin plugin;

	private readonly TicketListWindow ticketListWindow;

	private int ticketCount = 1;

	private string playerName = "";

	private string macroTemplate = "/shout We're running a 50/50 Raffle! The pot is at $pot , there are $bonusticketsleft buy one get one tickets left! Tickets cost 100K each!";

	private bool useManualBonus;

	private int manualBonus;

	private bool applyBogoThisEntry = true;

	private string ticketRangeTemplate = "/tell {PlayerName} Your tickets are %ticketrange";

	private List<string> discordChunks = new List<string>();

	private readonly RafflerLiteWindow? liteWindow;

	[PluginService]
	internal static ICommandManager CommandManager { get; private set; }

	private List<string> SplitIntoChunks(string fullMessage, int maxChunkLength = 1900)
	{
		List<string> list = new List<string>();
		string[] array = fullMessage.Split('\n');
		string text = "";
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (text.Length + text2.Length + 1 > maxChunkLength)
			{
				list.Add(text.TrimEnd());
				text = "";
			}
			text = text + text2 + "\n";
		}
		if (!string.IsNullOrWhiteSpace(text))
		{
			list.Add(text.TrimEnd());
		}
		return list;
	}

	public void UpdateDiscordChunks()
	{
		int activeStartMil = GetActiveStartMil();
		string item = $"RAFFLE {DateTime.Now:MM/dd/yy} - {activeStartMil}MIL STARTING POT";
		List<string> list = new List<string> { item };
		int num = 1;
		foreach (TicketEntry entry in plugin.Entries)
		{
			for (int i = 0; i < entry.TotalTickets; i++)
			{
				list.Add($"{num++,3}  {entry.PlayerName}");
			}
		}
		string fullMessage = string.Join("\n", list);
		discordChunks = SplitIntoChunks(fullMessage);
	}

	public MainWindow(Plugin plugin, string raffleImgArg, TicketListWindow ticketListWindow, RafflerLiteWindow? liteWindow)
		: base("Raffler##Main", (ImGuiWindowFlags)24, false)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		this.plugin = plugin;
		this.ticketListWindow = ticketListWindow;
		this.liteWindow = liteWindow;
		rafflerImg = raffleImgArg;
		((Window)this).Size = new Vector2(500f, 600f);
		((Window)this).SizeCondition = (ImGuiCond)4;
		WindowSizeConstraints value = default(WindowSizeConstraints);
		((WindowSizeConstraints)(ref value))._002Ector();
		((WindowSizeConstraints)(ref value)).MinimumSize = new Vector2(500f, 200f);
		((WindowSizeConstraints)(ref value)).MaximumSize = new Vector2(500f, float.MaxValue);
		((Window)this).SizeConstraints = value;
		this.plugin = plugin;
		this.ticketListWindow = ticketListWindow;
		ticketRangeTemplate = plugin.Configuration.TicketRangeTemplate ?? ticketRangeTemplate;
		applyBogoThisEntry = plugin.Configuration.DefaultApplyBogoForEntry;
		if (!string.IsNullOrWhiteSpace(plugin.Configuration.VenueMacro))
		{
			macroTemplate = plugin.Configuration.VenueMacro;
		}
	}

	public void Dispose()
	{
	}

	public override void Draw()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_188a: Unknown result type (might be due to invalid IL or missing references)
		//IL_188f: Unknown result type (might be due to invalid IL or missing references)
		//IL_189d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1963: Unknown result type (might be due to invalid IL or missing references)
		//IL_1986: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1038: Unknown result type (might be due to invalid IL or missing references)
		//IL_103d: Unknown result type (might be due to invalid IL or missing references)
		//IL_109a: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1344: Unknown result type (might be due to invalid IL or missing references)
		//IL_1358: Unknown result type (might be due to invalid IL or missing references)
		//IL_136c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1380: Unknown result type (might be due to invalid IL or missing references)
		//IL_1399: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_13bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1474: Unknown result type (might be due to invalid IL or missing references)
		//IL_1615: Unknown result type (might be due to invalid IL or missing references)
		//IL_1531: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15da: Unknown result type (might be due to invalid IL or missing references)
		//IL_1638: Unknown result type (might be due to invalid IL or missing references)
		//IL_163d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1654: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_173e: Unknown result type (might be due to invalid IL or missing references)
		IEndObject val = ImRaii.Child(ImU8String.op_Implicit("Content"), Vector2.Zero, true);
		try
		{
			if (!val.Success)
			{
				return;
			}
			ImGui.TextUnformatted(ImU8String.op_Implicit("Welcome to Raffler!"));
			IDalamudTextureWrap wrapOrDefault = Plugin.TextureProvider.GetFromFile(rafflerImg).GetWrapOrDefault((IDalamudTextureWrap)null);
			_ = ((wrapOrDefault != null) ? new ImTextureID?(wrapOrDefault.Handle) : ((ImTextureID?)null)).HasValue;
			ImGuiHelpers.ScaledDummy(20f);
			ImGui.Separator();
			ImGui.TextUnformatted(ImU8String.op_Implicit("\ue035 Raffle Settings"));
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit("\ue040 Pay Summary"), default(Vector2)))
			{
				plugin.TogglePaySummaryUI();
			}
			Configuration config = plugin.Configuration;
			if (ImGui.BeginCombo(ImU8String.op_Implicit("BOGO Type"), ImU8String.op_Implicit(config.RaffleBogoType.ToString()), (ImGuiComboFlags)0))
			{
				BogoType[] values = Enum.GetValues<BogoType>();
				for (int i = 0; i < values.Length; i++)
				{
					BogoType bogoType = values[i];
					if (ImGui.Selectable(ImU8String.op_Implicit(bogoType.ToString()), bogoType == config.RaffleBogoType, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						config.RaffleBogoType = bogoType;
						config.Save();
						plugin.SyncSession();
					}
				}
				ImGui.EndCombo();
			}
			bool num = plugin.Entries.Count > 0;
			bool countBonusTicketsTowardPot = config.CountBonusTicketsTowardPot;
			if (ImGui.Checkbox(ImU8String.op_Implicit("Count paid bonus tickets toward pot"), ref countBonusTicketsTowardPot))
			{
				config.CountBonusTicketsTowardPot = countBonusTicketsTowardPot;
				config.Save();
				plugin.SyncSession();
			}
			bool countVipTicketsTowardPot = config.CountVipTicketsTowardPot;
			if (ImGui.Checkbox(ImU8String.op_Implicit("Count VIP freebies toward pot"), ref countVipTicketsTowardPot))
			{
				config.CountVipTicketsTowardPot = countVipTicketsTowardPot;
				config.Save();
				plugin.SyncSession();
			}
			if (num)
			{
				ImGui.BeginDisabled();
			}
			ImGui.SetNextItemWidth(180f);
			int bogoSessionLimit = config.BogoSessionLimit;
			ImU8String val2 = ImU8String.op_Implicit("BOGO Session Limit");
			ImU8String val3 = default(ImU8String);
			if (ImGui.InputInt(val2, ref bogoSessionLimit, 0, 0, val3, (ImGuiInputTextFlags)0))
			{
				config.BogoSessionLimit = Math.Max(0, bogoSessionLimit);
				config.Save();
				plugin.SyncSession();
			}
			ImGui.SetNextItemWidth(180f);
			float num2 = config.TicketCost / 1000f;
			if (ImGui.InputFloat(ImU8String.op_Implicit("Ticket Cost (k)"), ref num2, 1f, 5f, ImU8String.op_Implicit("%.0f"), (ImGuiInputTextFlags)0))
			{
				config.TicketCost = num2 * 1000f;
				config.Save();
				plugin.SyncSession();
			}
			if (num)
			{
				ImGui.EndDisabled();
			}
			Vector4 vector;
			if (num)
			{
				vector = new Vector4(1f, 0.6f, 0.3f, 1f);
				ImGui.TextColored(ref vector, ImU8String.op_Implicit("\ue04c Bonus Tickets locked after first entry \ue04c"));
			}
			if (num)
			{
				ImGui.BeginDisabled();
			}
			bool num3 = plugin.Entries.Count > 0 || plugin.Configuration.ActiveSessionStartingPotMillions.HasValue;
			int activeStartMil = GetActiveStartMil();
			ImGui.SetNextItemWidth(180f);
			if (num3)
			{
				ImGui.BeginDisabled();
				ImU8String val4 = ImU8String.op_Implicit("Starting Pot (mil)");
				val3 = default(ImU8String);
				ImGui.InputInt(val4, ref activeStartMil, 0, 0, val3, (ImGuiInputTextFlags)0);
				ImGui.EndDisabled();
			}
			else
			{
				ImU8String val5 = ImU8String.op_Implicit("Starting Pot (mil)");
				val3 = default(ImU8String);
				if (ImGui.InputInt(val5, ref activeStartMil, 0, 0, val3, (ImGuiInputTextFlags)0))
				{
					plugin.Configuration.StartingPotMillions = Math.Max(0, activeStartMil);
					plugin.Configuration.Save();
					plugin.SyncSession();
				}
			}
			if (num)
			{
				ImGui.EndDisabled();
			}
			ImGui.Separator();
			bool showWorldNameInTicket = plugin.Configuration.ShowWorldNameInTicket;
			if (ImGui.Checkbox(ImU8String.op_Implicit("Show Server Name"), ref showWorldNameInTicket))
			{
				plugin.Configuration.ShowWorldNameInTicket = showWorldNameInTicket;
				plugin.Configuration.Save();
				plugin.SyncSession();
			}
			ImGui.Separator();
			ImGui.TextUnformatted(ImU8String.op_Implicit("\ue035 Ticket Entry"));
			if (ImGui.Checkbox(ImU8String.op_Implicit("Apply BOGO for this entry"), ref applyBogoThisEntry))
			{
				plugin.Configuration.DefaultApplyBogoForEntry = applyBogoThisEntry;
				plugin.Configuration.Save();
				plugin.SyncSession();
			}
			ImGui.Checkbox(ImU8String.op_Implicit("VIP Bogo"), ref useManualBonus);
			ImGui.SameLine();
			ImGui.SetNextItemWidth(100f);
			ImU8String val6 = ImU8String.op_Implicit("Bonus");
			ref int reference = ref manualBonus;
			val3 = default(ImU8String);
			ImGui.InputInt(val6, ref reference, 0, 0, val3, (ImGuiInputTextFlags)0);
			manualBonus = Math.Max(0, manualBonus);
			ImGui.InputText(ImU8String.op_Implicit("Player Name"), ref playerName, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.Button(ImU8String.op_Implicit("@ Target"), default(Vector2)))
			{
				IGameObject target = Plugin.TargetManager.Target;
				IPlayerCharacter val7 = (IPlayerCharacter)(object)((target is IPlayerCharacter) ? target : null);
				if (val7 != null)
				{
					string textValue = ((IGameObject)val7).Name.TextValue;
					World value = val7.HomeWorld.Value;
					string text = ((object)((World)(ref value)).Name/*cast due to .constrained prefix*/).ToString();
					playerName = (showWorldNameInTicket ? (textValue + "@" + text) : textValue);
				}
			}
			ImU8String val8 = ImU8String.op_Implicit("Tickets Requested");
			ref int reference2 = ref ticketCount;
			val3 = default(ImU8String);
			ImGui.InputInt(val8, ref reference2, 1, 10, val3, (ImGuiInputTextFlags)0);
			ticketCount = Math.Clamp(ticketCount, 1, 10000);
			_ = ticketCount;
			_ = config.TicketCost;
			switch (config.RaffleBogoType)
			{
			case BogoType.None:
			{
				int i = 0;
				break;
			}
			case BogoType.Buy1Get1:
			{
				int i = Math.Min(ticketCount, plugin.BonusTicketsRemaining);
				break;
			}
			default:
			{
				int i = 0;
				break;
			}
			}
			int val9 = ((plugin.Entries.Count > 0 || plugin.Configuration.ActiveSessionStartingPotMillions.HasValue) ? Math.Max(0, plugin.BonusTicketsRemaining) : Math.Max(0, plugin.Configuration.BogoSessionLimit));
			int num4 = ((applyBogoThisEntry && config.RaffleBogoType == BogoType.Buy1Get1) ? Math.Min(ticketCount, val9) : 0);
			float num5 = (float)ticketCount * config.TicketCost;
			int num6 = 0;
			if (useManualBonus)
			{
				num6 = Math.Max(0, manualBonus);
			}
			else
			{
				string text2 = playerName.Trim();
				if (!string.IsNullOrWhiteSpace(text2) && plugin.Configuration.VipRules.TryGetValue(text2, out Configuration.VipRule value2))
				{
					int used = plugin.Vip.GetUsed(text2);
					int val10 = Math.Max(0, value2.FirstNFreeTickets - used);
					num6 = Math.Min(ticketCount, val10);
				}
			}
			int num7 = ticketCount + num4 + num6;
			ImGui.Separator();
			val3 = new ImU8String(12, 1);
			((ImU8String)(ref val3)).AppendLiteral("\ue06f Cost: ");
			((ImU8String)(ref val3)).AppendFormatted<float>(num5, "N0");
			((ImU8String)(ref val3)).AppendLiteral(" gil");
			ImGui.TextUnformatted(val3);
			ImGui.SameLine();
			val3 = new ImU8String(27, 2);
			((ImU8String)(ref val3)).AppendLiteral("\ue04e Bonus Tickets: ");
			((ImU8String)(ref val3)).AppendFormatted<int>(num4);
			((ImU8String)(ref val3)).AppendLiteral("  |  VIP: ");
			((ImU8String)(ref val3)).AppendFormatted<int>(num6);
			ImGui.TextUnformatted(val3);
			ImGui.SameLine();
			val3 = new ImU8String(17, 1);
			((ImU8String)(ref val3)).AppendLiteral("\ue06f Total Tickets: ");
			((ImU8String)(ref val3)).AppendFormatted<int>(num7);
			ImGui.TextUnformatted(val3);
			if (ImGui.Button(ImU8String.op_Implicit("\ue06f Confirm Entry"), default(Vector2)))
			{
				if (!string.IsNullOrWhiteSpace(playerName))
				{
					if (plugin.Entries.Any((TicketEntry e) => string.Equals(Canon(e.PlayerName), Canon(playerName), StringComparison.OrdinalIgnoreCase)))
					{
						showRepeatBuyerWarning = true;
					}
					else
					{
						allowRepeatBuyer = true;
					}
				}
				else
				{
					vector = new Vector4(1f, 0.2f, 0.2f, 1f);
					ImGui.TextColored(ref vector, ImU8String.op_Implicit("\ue0bf Please enter a player name."));
				}
			}
			if (showRepeatBuyerWarning)
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("Repeat Buyer?"), (ImGuiPopupFlags)0);
				showRepeatBuyerWarning = false;
			}
			bool flag = true;
			if (ImGui.BeginPopupModal(ImU8String.op_Implicit("Repeat Buyer?"), ref flag, (ImGuiWindowFlags)64))
			{
				val3 = new ImU8String(68, 1);
				((ImU8String)(ref val3)).AppendLiteral("\ue0bf ");
				((ImU8String)(ref val3)).AppendFormatted<string>(playerName);
				((ImU8String)(ref val3)).AppendLiteral(" has already entered.\nAre you sure you want to add more tickets? \ue0bf");
				ImGui.Text(val3);
				ImGui.Spacing();
				if (ImGui.Button(ImU8String.op_Implicit("Yes, Allow"), default(Vector2)))
				{
					allowRepeatBuyer = true;
					ImGui.CloseCurrentPopup();
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit("Cancel"), default(Vector2)))
				{
					allowRepeatBuyer = false;
					ImGui.CloseCurrentPopup();
				}
				ImGui.EndPopup();
			}
			if (allowRepeatBuyer)
			{
				allowRepeatBuyer = false;
				if (plugin.Entries.Count == 0)
				{
					plugin.Configuration.ActiveSessionStartingPotMillions = plugin.Configuration.StartingPotMillions;
					plugin.Configuration.Save();
					plugin.SessionStartTime = DateTime.Now;
					plugin.BonusTicketsRemaining = plugin.Configuration.BogoSessionLimit;
					plugin.SaveEntries();
					UpdateDiscordChunks();
					ticketListWindow.UpdateDiscordChunks(plugin.Entries, $"RAFFLE {DateTime.Now:MM/dd/yy} - {plugin.Configuration.StartingPotMillions}MIL STARTING POT");
				}
				int num8 = 0;
				if (applyBogoThisEntry && plugin.Configuration.RaffleBogoType == BogoType.Buy1Get1)
				{
					num8 = Math.Min(val2: (plugin.Entries.Count == 0 && !plugin.Configuration.ActiveSessionStartingPotMillions.HasValue) ? Math.Max(0, plugin.Configuration.BogoSessionLimit) : Math.Max(0, plugin.BonusTicketsRemaining), val1: ticketCount);
				}
				int freeTickets = 0;
				string text3 = playerName.Trim();
				Configuration.VipRule value3;
				if (useManualBonus)
				{
					freeTickets = Math.Max(0, manualBonus);
				}
				else if (plugin.Configuration.VipRules.TryGetValue(text3, out value3))
				{
					int used2 = plugin.Vip.GetUsed(text3);
					int num9 = Math.Max(0, value3.FirstNFreeTickets - used2);
					if (num9 > 0)
					{
						int num10 = Math.Min(ticketCount, num9);
						freeTickets = num10;
						plugin.Vip.AddUsed(text3, num10);
					}
				}
				TicketEntry ticketEntry = new TicketEntry
				{
					PlayerName = playerName.Trim(),
					BaseTickets = ticketCount,
					BonusTickets = num8,
					FreeTickets = freeTickets
				};
				plugin.Entries.Add(ticketEntry);
				plugin.BonusTicketsRemaining = Math.Max(0, plugin.BonusTicketsRemaining - num8);
				plugin.SaveEntries();
				useManualBonus = false;
				manualBonus = 0;
				applyBogoThisEntry = plugin.Configuration.RaffleBogoType != BogoType.None;
				int activeStartMil2 = GetActiveStartMil();
				ticketListWindow.UpdateDiscordChunks(plugin.Entries, $"RAFFLE {DateTime.Now:MM/dd/yy} - {activeStartMil2}MIL STARTING POT");
				int totalTickets = ticketEntry.TotalTickets;
				int num11 = plugin.Entries.Sum((TicketEntry e) => e.BaseTickets + e.BonusTickets + e.FreeTickets) - totalTickets;
				int num12 = num11 + 1;
				int num13 = num11 + totalTickets;
				Plugin.ChatGui.Print($"Ticket Entry: {ticketEntry.PlayerName} - (Numbers {num12}-{num13})", (string)null, (ushort?)null);
				SendAutoTell(ticketEntry, num12, num13);
				List<string> chunks = BuildTicketChunksForEntry(ticketEntry, num12);
				Task.Run(async delegate
				{
					foreach (string item in chunks)
					{
						try
						{
							if (!string.IsNullOrWhiteSpace(plugin.Configuration.DiscordWebhookUrl))
							{
								await DiscordWebhookHelper.SendTextToDiscord(plugin.Configuration.DiscordWebhookUrl, item);
							}
						}
						catch (Exception ex2)
						{
							Plugin.Log.Error("❌ Failed to send Discord chunk: " + ex2.Message, Array.Empty<object>());
						}
						await Task.Delay(100);
					}
				});
			}
			ImGui.Separator();
			if (ImGui.Button(ImU8String.op_Implicit("\ue05a Current Ticket List"), default(Vector2)))
			{
				plugin.TicketListUI();
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit("\ue0bf Reset Raffle Session"), default(Vector2)))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("Confirm Reset?"), (ImGuiPopupFlags)0);
			}
			bool flag2 = true;
			if (ImGui.BeginPopupModal(ImU8String.op_Implicit("Confirm Reset?"), ref flag2, (ImGuiWindowFlags)64))
			{
				ImGui.Text(ImU8String.op_Implicit("Are you sure you want to reset the current raffle?"));
				ImGui.Spacing();
				if (ImGui.Button(ImU8String.op_Implicit("Yes, Reset"), default(Vector2)))
				{
					plugin.Entries.Clear();
					plugin.BonusTicketsRemaining = plugin.Configuration.BogoBonusTickets;
					plugin.Configuration.StartingGil = 0;
					plugin.Configuration.ActiveSessionStartingPotMillions = null;
					plugin.Configuration.Save();
					plugin.SaveEntries();
					plugin.SessionStartTime = DateTime.Now;
					ticketListWindow.ClearDiscordChunks();
					ImGui.CloseCurrentPopup();
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit("Cancel"), default(Vector2)))
				{
					ImGui.CloseCurrentPopup();
				}
				ImGui.EndPopup();
			}
			int num14 = plugin.Entries.Sum((TicketEntry e) => e.FreeTickets);
			int num15 = plugin.Entries.Sum((TicketEntry e) => e.BaseTickets);
			int num16 = plugin.Entries.Sum((TicketEntry e) => e.BonusTickets);
			bool countBonusTicketsTowardPot2 = plugin.Configuration.CountBonusTicketsTowardPot;
			bool countVipTicketsTowardPot2 = plugin.Configuration.CountVipTicketsTowardPot;
			int num17 = num15 + num16 + num14;
			int num18 = num15 + (countBonusTicketsTowardPot2 ? num16 : 0) + (countVipTicketsTowardPot2 ? num14 : 0);
			float num19 = (float)num15 * plugin.Configuration.TicketCost;
			float num20 = (float)num18 * plugin.Configuration.TicketCost;
			float num21 = (float)GetActiveStartMil() * 1000000f;
			float num22 = (config.UseFiftyFiftySplit ? (num21 / 2f) : num21);
			float num23 = num21 + num20;
			float num24 = num22 + num19;
			float num25 = num23 / 2f;
			float g = num24 - num25 - num22;
			vector = new Vector4(1f, 0.4f, 0.4f, 1f);
			val3 = new ImU8String(64, 3);
			((ImU8String)(ref val3)).AppendLiteral(" Tickets Issued: ");
			((ImU8String)(ref val3)).AppendFormatted<int>(num17);
			((ImU8String)(ref val3)).AppendLiteral(" · Gil collected: ");
			((ImU8String)(ref val3)).AppendFormatted<float>(num19, "N0");
			((ImU8String)(ref val3)).AppendLiteral(" · Pot: ");
			((ImU8String)(ref val3)).AppendFormatted<float>(num23 / 1000000f, "N1");
			((ImU8String)(ref val3)).AppendLiteral("m - Hover for details");
			ImGui.TextColored(ref vector, val3);
			if (ImGui.IsItemHovered())
			{
				ImGui.BeginTooltip();
				vector = new Vector4(0.9f, 0.9f, 1f, 1f);
				ImGui.TextColored(ref vector, ImU8String.op_Implicit("Pot overview"));
				ImGui.Separator();
				if (ImGui.BeginTable(ImU8String.op_Implicit("##pot_breakdown"), 2, (ImGuiTableFlags)24576, default(Vector2), 0f))
				{
					Row("Paid tickets", num15.ToString("N0") + " (counted)");
					Row("BOGO tickets", $"{num16:N0}" + (countBonusTicketsTowardPot2 ? " (counted)" : " (not counted)"));
					Row("VIP free tickets", $"{num14:N0}" + (countVipTicketsTowardPot2 ? " (counted)" : " (not counted)"));
					Row("Tickets issued", num17.ToString("N0"));
					Row("Tickets counted toward pot", num18.ToString("N0"));
					Row("Gil collected (sold)", Money(num19));
					if (Math.Abs(num20 - num19) > 0.5f || countBonusTicketsTowardPot2 || countVipTicketsTowardPot2)
					{
						Row("Gil added to pot", Money(num20));
					}
					Row("Advertised start", Money(num21));
					if (config.UseFiftyFiftySplit && Math.Abs(num22 - num21) > 0.5f)
					{
						Row("Starting funds", Money(num22));
					}
					Row("Advertised pot", Money(num23));
					if (Math.Abs(num24 - num23) > 0.5f)
					{
						Row("Payout pot (starting funds + sales)", Money(num24));
					}
					if (config.UseFiftyFiftySplit)
					{
						Row("Winner (50% of advertised)", Money(num25));
						Row("Venue take-home", Money(g));
					}
					ImGui.EndTable();
				}
				ImGui.EndTooltip();
			}
			if (ImGui.InputTextMultiline(ImU8String.op_Implicit("\ue0bb Macro Template"), ref macroTemplate, 256, new Vector2(-1f, ImGui.GetTextLineHeight() * 2f), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
			{
				plugin.Configuration.VenueMacro = macroTemplate;
				plugin.Configuration.Save();
				plugin.SyncSession();
			}
			vector = new Vector4(0.2f, 1f, 0.2f, 1f);
			ImGui.TextColored(ref vector, ImU8String.op_Implicit("Variables:"));
			ImGui.SameLine();
			ImGui.Text(ImU8String.op_Implicit("$pot"));
			ImGui.SameLine();
			ImGui.Text(ImU8String.op_Implicit("$ticketsold"));
			ImGui.SameLine();
			ImGui.Text(ImU8String.op_Implicit("$bonusticketsleft"));
			ImGui.NewLine();
			ImGui.SameLine();
			ImGui.Text(ImU8String.op_Implicit("$gilmade (sales only)"));
			ImGui.SameLine();
			ImGui.Text(ImU8String.op_Implicit("$winnerpot (50% of pot)"));
			if (ImGui.Button(ImU8String.op_Implicit("\ue040 Copy Venue Raffle Macro"), default(Vector2)))
			{
				ImGui.SetClipboardText(ImU8String.op_Implicit(macroTemplate.Replace("$pot", (num23 / 1000000f).ToString("N1") + "m").Replace("$ticketsold", num17.ToString()).Replace("$bonusticketsleft", plugin.BonusTicketsRemaining.ToString())
					.Replace("$gilmade", num19.ToString("N0"))
					.Replace("$winnerpot", (num23 / 2000000f).ToString("N1") + "m")));
			}
			ImGui.Separator();
			ImGui.Spacing();
			vector = new Vector4(1f, 0.95f, 0.35f, 1f);
			ImGui.TextColored(ref vector, ImU8String.op_Implicit("★ Tell Automation ★"));
			bool enableTicketRangeTells = plugin.Configuration.EnableTicketRangeTells;
			if (ImGui.Checkbox(ImU8String.op_Implicit("Enable automatic ticket range tells"), ref enableTicketRangeTells))
			{
				plugin.Configuration.EnableTicketRangeTells = enableTicketRangeTells;
				plugin.Configuration.Save();
				plugin.SyncSession();
			}
			if (enableTicketRangeTells)
			{
				ImGui.PushStyleColor((ImGuiCol)7, new Vector4(1f, 0.95f, 0.35f, 0.5f));
				if (ImGui.InputTextMultiline(ImU8String.op_Implicit("\ue0bb Ticket Range Macro"), ref ticketRangeTemplate, 256, new Vector2(-1f, ImGui.GetTextLineHeight() * 2f), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
				{
					plugin.Configuration.TicketRangeTemplate = ticketRangeTemplate;
					plugin.Configuration.Save();
					plugin.SyncSession();
				}
				vector = new Vector4(1f, 0.95f, 0.35f, 1f);
				ImGui.TextColored(ref vector, ImU8String.op_Implicit("Variables:"));
				ImGui.SameLine();
				ImGui.Text(ImU8String.op_Implicit("%ticketrange"));
				ImGui.SameLine();
				ImGui.Text(ImU8String.op_Implicit("{PlayerName}"));
				ImGui.PopStyleColor();
			}
			ImGui.Separator();
			ImGui.Separator();
			vector = new Vector4(0.2f, 1f, 0.2f, 1f);
			ImGui.TextColored(ref vector, ImU8String.op_Implicit("\ue03d Advanced Tools"));
			string text4 = (showFileTools ? "\ue039" : "\ue039");
			val3 = new ImU8String(37, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(text4);
			((ImU8String)(ref val3)).AppendLiteral(" File Operations ##ToggleDiscordTools");
			if (ImGui.Button(val3, new Vector2(-1f, 0f)))
			{
				showFileTools = !showFileTools;
			}
			if (showFileTools)
			{
				ImGui.Spacing();
				vector = new Vector4(0.2f, 1f, 0.2f, 1f);
				ImGui.TextColored(ref vector, ImU8String.op_Implicit("\ue039 File Operations"));
				if (ImGui.Button(ImU8String.op_Implicit("\ue039 Load Tickets from File"), default(Vector2)))
				{
					string initialPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
					filePicker.Open("ChooseRaffleCSV", initialPath, delegate(string selectedFile)
					{
						pendingCsvPath = selectedFile;
						importStartingPot = plugin.Configuration.StartingPotMillions;
						importBogoType = plugin.Configuration.RaffleBogoType;
						showImportSettingsPopup = true;
					});
				}
				if (ImGuiFileDialog.Display("ChooseRaffleCSV"))
				{
					if (ImGuiFileDialog.IsOk())
					{
						string filePathName = ImGuiFileDialog.GetFilePathName();
						plugin.CsvImporter.LoadCsvFromFile(filePathName);
					}
					ImGuiFileDialog.Close();
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit("\ue03d Save Tickets to Downloads"), default(Vector2)))
				{
					try
					{
						string contents = plugin.GenerateCsvFromTickets();
						string text5 = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"), $"raffle_tickets_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
						File.WriteAllText(text5, contents);
						Plugin.ChatGui.Print("✅ Tickets saved to " + text5, (string)null, (ushort?)null);
					}
					catch (Exception ex)
					{
						Plugin.ChatGui.PrintError("❌ Failed to save CSV to Desktop.", (string)null, (ushort?)null);
						Plugin.Log.Error("Error saving to desktop: " + ex.Message, Array.Empty<object>());
					}
				}
			}
			_ = showDiscordTools;
			ImGui.PushStyleColor((ImGuiCol)21, new Vector4(0.2f, 0.5f, 0.6f, 1f));
			ImGui.PushStyleColor((ImGuiCol)22, new Vector4(0.3f, 0.6f, 0.7f, 1f));
			ImGui.PushStyleColor((ImGuiCol)23, new Vector4(0.1f, 0.4f, 0.5f, 1f));
			val3 = new ImU8String(35, 0);
			((ImU8String)(ref val3)).AppendLiteral("\ue048 Discord Tools##ToggleDiscordTools");
			if (ImGui.Button(val3, new Vector2(-1f, 0f)))
			{
				showDiscordTools = !showDiscordTools;
			}
			ImGui.PopStyleColor(3);
			if (showDiscordTools)
			{
				ImGui.Spacing();
				vector = new Vector4(0.5f, 1f, 1f, 1f);
				ImGui.TextColored(ref vector, ImU8String.op_Implicit("\ue0bd Discord Webhook"));
				Vector4 vector2 = new Vector4(0f, 1f, 1f, 1f);
				string text6 = config.DiscordWebhookUrl ?? "";
				ImGui.PushStyleColor((ImGuiCol)7, new Vector4(0.05f, 0.15f, 0.18f, 1f));
				ImGui.PushStyleColor((ImGuiCol)0, vector2);
				ImGui.TextUnformatted(ImU8String.op_Implicit("Webhook URL"));
				ImGui.PopStyleColor();
				ImGui.SameLine();
				ImGui.SetNextItemWidth(-1f);
				if (ImGui.InputText(ImU8String.op_Implicit("##WebhookURL"), ref text6, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null) && text6 != config.DiscordWebhookUrl)
				{
					config.DiscordWebhookUrl = text6.Trim();
					config.Save();
					Plugin.ChatGui.Print("✅ Webhook saved.", (string)null, (ushort?)null);
					plugin.SyncSession();
				}
				ImGui.PopStyleColor();
				if (ImGui.Button(ImU8String.op_Implicit("\ue0bd Send Tickets to Discord"), default(Vector2)))
				{
					Task.Run(async delegate
					{
						try
						{
							string csvContent = plugin.GenerateCsvFromTickets();
							if (!string.IsNullOrWhiteSpace(config.DiscordWebhookUrl))
							{
								await DiscordWebhookHelper.SendCsvToDiscord(config.DiscordWebhookUrl, "raffle_tickets.csv", csvContent);
							}
							else
							{
								Plugin.ChatGui.PrintError("❌ No Discord webhook URL set in config.", (string)null, (ushort?)null);
							}
							Plugin.ChatGui.Print("\ue0bd Tickets successfully sent to Discord!", (string)null, (ushort?)null);
						}
						catch (Exception ex2)
						{
							Plugin.Log.Error("Failed to send to Discord: " + ex2.Message, Array.Empty<object>());
							Plugin.ChatGui.PrintError("❌ Failed to send tickets to Discord.", (string)null, (ushort?)null);
						}
					});
				}
				RafflerTheme.Pop();
			}
			RafflerTheme.Pop();
			filePicker.Draw();
			if (showImportSettingsPopup)
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("Import Settings"), (ImGuiPopupFlags)0);
				showImportSettingsPopup = false;
			}
			bool flag3 = true;
			if (!ImGui.BeginPopupModal(ImU8String.op_Implicit("Import Settings"), ref flag3, (ImGuiWindowFlags)64))
			{
				return;
			}
			ImGui.Text(ImU8String.op_Implicit("Configure settings before importing raffle tickets:"));
			ImGui.Separator();
			ImU8String val11 = ImU8String.op_Implicit("\ue0bb Starting Pot (mil)");
			ref int reference3 = ref importStartingPot;
			val3 = default(ImU8String);
			ImGui.InputInt(val11, ref reference3, 0, 0, val3, (ImGuiInputTextFlags)0);
			if (ImGui.BeginCombo(ImU8String.op_Implicit("\ue0bb BOGO Type"), ImU8String.op_Implicit(importBogoType.ToString()), (ImGuiComboFlags)0))
			{
				BogoType[] values = Enum.GetValues<BogoType>();
				for (int i = 0; i < values.Length; i++)
				{
					BogoType bogoType2 = values[i];
					if (ImGui.Selectable(ImU8String.op_Implicit(bogoType2.ToString()), bogoType2.Equals(importBogoType), (ImGuiSelectableFlags)0, default(Vector2)))
					{
						importBogoType = bogoType2;
					}
				}
				ImGui.EndCombo();
			}
			ImGui.Spacing();
			if (ImGui.Button(ImU8String.op_Implicit("✅ Import"), default(Vector2)))
			{
				plugin.Configuration.StartingPotMillions = Math.Max(0, importStartingPot);
				plugin.Configuration.RaffleBogoType = importBogoType;
				plugin.Configuration.Save();
				plugin.CsvImporter.LoadCsvFromFile(pendingCsvPath);
				ImGui.CloseCurrentPopup();
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit("❌ Cancel"), default(Vector2)))
			{
				pendingCsvPath = "";
				ImGui.CloseCurrentPopup();
			}
			ImGui.EndPopup();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		static string Money(float num26)
		{
			if (!(num26 >= 1000000f))
			{
				return num26.ToString("N0") + " gil";
			}
			return (num26 / 1000000f).ToString("N1") + "m gil";
		}
		static void Row(string k, string v)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(k));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(v));
		}
	}

	private static string Canon(string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return "";
		}
		string text = s.Trim();
		int num = text.IndexOf('@');
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		int num2 = text.LastIndexOf(" - ", StringComparison.Ordinal);
		if (num2 >= 0)
		{
			text = text.Substring(0, num2);
		}
		return text;
	}

	private void SendAutoTell(TicketEntry entry, int startTicket, int endTicket)
	{
		if (!plugin.Configuration.EnableTicketRangeTells)
		{
			return;
		}
		string text = playerName.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			Plugin.ChatGui.PrintError("❌ No player in the Player Name field for /tell.", (string)null, (ushort?)null);
			return;
		}
		string newValue = ((entry.TotalTickets == 1) ? $"{startTicket}" : $"{startTicket}-{endTicket}");
		string text2 = ticketRangeTemplate.Replace("{PlayerName}", text).Replace("%ticketrange", newValue);
		string text3 = text2.TrimStart();
		if (text3.Length > 0 && text3[0] == '/')
		{
			Chat.SendMessage(text2);
		}
		else
		{
			Plugin.ChatGui.PrintError("❌ Ticket range macro must start with a slash (e.g., /tell).", (string)null, (ushort?)null);
		}
	}

	private List<string> BuildTicketChunksForEntry(TicketEntry entry, int startNumber, int maxChunkLength = 1900)
	{
		List<string> list = new List<string>();
		string text = "";
		int num = startNumber;
		int totalTickets = entry.TotalTickets;
		int num2 = 0;
		while (num2 < totalTickets)
		{
			string text2 = $"{num,3}  {entry.PlayerName}\n";
			if (text.Length + text2.Length > maxChunkLength)
			{
				list.Add(text.TrimEnd());
				text = "";
			}
			text += text2;
			num2++;
			num++;
		}
		if (!string.IsNullOrWhiteSpace(text))
		{
			list.Add(text.TrimEnd());
		}
		return list;
	}

	private int GetActiveStartMil()
	{
		return plugin.Configuration.ActiveSessionStartingPotMillions ?? plugin.Configuration.StartingPotMillions;
	}
}
