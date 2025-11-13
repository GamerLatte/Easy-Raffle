using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Windowing;
using EasyRaffle.Data;

namespace EasyRaffle.Windows;

public class EasyRaffleLiteWindow : Window, IDisposable
{
	private readonly Plugin plugin;

	private readonly MainWindow mainWindow;

	private readonly TicketListWindow ticketListWindow;

	private List<string> discordChunks = new List<string>();

	private int ticketCount = 1;

	private string playerName = "";

	private bool showRepeatBuyerWarning;

	private bool allowRepeatBuyer;

	public EasyRaffleLiteWindow(Plugin plugin, MainWindow mainWindow, TicketListWindow ticketListWindow)
		: base("Easy Raffle Lite", (ImGuiWindowFlags)24, false)
	{
		this.plugin = plugin;
		this.mainWindow = mainWindow;
		this.ticketListWindow = ticketListWindow;
		((Window)this).Size = new Vector2(463f, 266f);
		((Window)this).SizeCondition = (ImGuiCond)1;
	}

	public void Dispose()
	{
	}

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
		string item = $"RAFFLE {DateTime.Now:MM/dd/yy} - {plugin.Configuration.StartingPotMillions}MIL STARTING POT";
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

	public override void Draw()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		ImGui.Text(ImU8String.op_Implicit("Player Name"));
		ImGui.SetNextItemWidth(-1f);
		ImGui.InputText(ImU8String.op_Implicit("##PlayerName"), ref playerName, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.Button(ImU8String.op_Implicit("@"), default(Vector2)))
		{
			IGameObject target = Plugin.TargetManager.Target;
			if (target != null && (int)target.ObjectKind == 1)
			{
				playerName = target.Name.TextValue;
			}
		}
		ImGui.SameLine();
		if (ImGui.Button(ImU8String.op_Implicit("/t"), default(Vector2)))
		{
			if (!string.IsNullOrWhiteSpace(plugin.LastTellTarget))
			{
				playerName = plugin.LastTellTarget;
			}
			else
			{
				Plugin.ChatGui.PrintError("❌ No recent /t target available.", (string)null, (ushort?)null);
			}
		}
		ImGui.Text(ImU8String.op_Implicit("Tickets Requested"));
		ImGui.SetNextItemWidth(120f);
		ImU8String val = ImU8String.op_Implicit("##TicketCount");
		ref int reference = ref ticketCount;
		ImU8String val2 = default(ImU8String);
		ImGui.InputInt(val, ref reference, 0, 0, val2, (ImGuiInputTextFlags)0);
		ticketCount = Math.Clamp(ticketCount, 1, 10000);
		int num = ticketCount;
		float num2 = (float)num * plugin.Configuration.TicketCost;
		((ImU8String)(ref val2))._002Ector(11, 2);
		((ImU8String)(ref val2)).AppendLiteral("\ue06f ");
		((ImU8String)(ref val2)).AppendFormatted<int>(num);
		((ImU8String)(ref val2)).AppendLiteral(" for ");
		((ImU8String)(ref val2)).AppendFormatted<float>(num2, "N0");
		((ImU8String)(ref val2)).AppendLiteral(" gil");
		ImGui.TextUnformatted(val2);
		if (ImGui.Button(ImU8String.op_Implicit("Confirm Entry"), default(Vector2)))
		{
			if (!string.IsNullOrWhiteSpace(playerName))
			{
				if (plugin.Entries.Exists((TicketEntry e) => e.PlayerName.Equals(playerName.Trim(), StringComparison.OrdinalIgnoreCase)))
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
				Plugin.ChatGui.PrintError("Please enter a player name.", (string)null, (ushort?)null);
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
			((ImU8String)(ref val2))._002Ector(66, 1);
			((ImU8String)(ref val2)).AppendLiteral("\ue0bf ");
			((ImU8String)(ref val2)).AppendFormatted<string>(playerName);
			((ImU8String)(ref val2)).AppendLiteral(" has already entered.\nAre you sure you want to add more tickets?");
			ImGui.Text(val2);
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
				plugin.Configuration.StartingGil = (int)((float)ticketCount * plugin.Configuration.TicketCost);
				plugin.SessionStartTime = DateTime.Now;
			}
			TicketEntry item = new TicketEntry
			{
				PlayerName = playerName.Trim(),
				BaseTickets = ticketCount,
				BonusTickets = 0
			};
			plugin.Entries.Add(item);
			plugin.SaveEntries();
			ticketListWindow.ClearDiscordChunks();
			ticketListWindow.GenerateDiscordChunks();
		}
		ImGui.Separator();
		ImGui.Spacing();
		if (ImGui.Button(ImU8String.op_Implicit("\ud83d\udd01 Return to Full Mode"), default(Vector2)))
		{
			Plugin.CommandManager.ProcessCommand("/raffler");
			((Window)this).IsOpen = false;
		}
		ImGui.SameLine();
		if (ImGui.Button(ImU8String.op_Implicit("\ud83d\udccb Open Ticket List"), default(Vector2)))
		{
			((Window)ticketListWindow).Toggle();
		}
	}
}
