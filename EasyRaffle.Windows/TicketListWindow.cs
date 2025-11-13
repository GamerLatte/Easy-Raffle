using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using EasyRaffle.Data;

namespace EasyRaffle.Windows;

public class TicketListWindow : Window, IDisposable
{
	private bool showDiscordView;

	private string discordHeader;

	private readonly Plugin plugin;

	private bool showDiscordChunks;

	private List<string> discordChunks = new List<string>();

	private int selectedIndex = -1;

	private string editName = "";

	private int editBase = 1;

	private int editBonus;

	private int editVIP;

	private bool showWorldTop;

	private bool showWorldSummary;

	private readonly HashSet<string> GoldenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Nilah Valoryn", "DJ Reigna", "♥Dj Reigna", "Nilah Valoryn@Golem", "Kiera Virelle@Golem", "Kiera Virelle" };

	private static string FormatDisplayName(string canonical, bool showWorld)
	{
		if (string.IsNullOrWhiteSpace(canonical))
		{
			return canonical;
		}
		if (!showWorld)
		{
			return canonical.Split('@')[0].Trim();
		}
		return canonical;
	}

	public TicketListWindow(Plugin plugin)
		: base("Raffle Tickets ###raffleTickets")
	{
		this.plugin = plugin;
		((Window)this).Flags = (ImGuiWindowFlags)0;
		((Window)this).Size = new Vector2(300f, 400f);
		((Window)this).SizeCondition = (ImGuiCond)4;
		int startingPotMillions = plugin.Configuration.StartingPotMillions;
		discordHeader = $"RAFFLE {DateTime.Now:M/d/yy} - {startingPotMillions}MIL STARTING POT";
		showWorldTop = plugin.Configuration.ShowWorldNameInTicket;
		showWorldSummary = plugin.Configuration.ShowWorldNameInTicket;
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

	public void ClearDiscordChunks()
	{
		showDiscordView = false;
		showDiscordChunks = false;
		discordChunks.Clear();
	}

	public void UpdateDiscordChunks(List<TicketEntry> entries, string headerText)
	{
		List<string> list = new List<string> { headerText };
		int num = 1;
		foreach (TicketEntry entry in entries)
		{
			for (int i = 0; i < entry.TotalTickets; i++)
			{
				list.Add($"{num++,3}  {entry.PlayerName}");
			}
		}
		string fullMessage = string.Join("\n", list);
		discordChunks = SplitIntoChunks(fullMessage);
	}

	public void Dispose()
	{
	}

	public void GenerateDiscordChunks()
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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		if (plugin.Entries.Count == 0)
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(" No entries yet!"));
		}
		else
		{
			if (!ImGui.BeginTabBar(ImU8String.op_Implicit("RaffleTabBar"), (ImGuiTabBarFlags)0))
			{
				return;
			}
			ImU8String val = default(ImU8String);
			if (ImGui.BeginTabItem(ImU8String.op_Implicit(" Raffle Tickets"), (ImGuiTabItemFlags)0))
			{
				((ImU8String)(ref val))._002Ector(18, 1);
				((ImU8String)(ref val)).AppendLiteral(" Raffle Entries (");
				((ImU8String)(ref val)).AppendFormatted<int>(plugin.Entries.Count);
				((ImU8String)(ref val)).AppendLiteral(")");
				ImGui.TextUnformatted(val);
				ImGui.SameLine();
				ImGui.Spacing();
				ImGui.SameLine();
				ImGui.Checkbox(ImU8String.op_Implicit("Show Server Name (List)"), ref showWorldTop);
				ImGui.Separator();
				ImGui.BeginChild(ImU8String.op_Implicit("TicketScroll"), new Vector2(0f, 250f), true, (ImGuiWindowFlags)0);
				for (int i = 0; i < plugin.Entries.Count; i++)
				{
					TicketEntry ticketEntry = plugin.Entries[i];
					string text = ComposeEntryLine(FormatDisplayName(ticketEntry.PlayerName, showWorldTop), ticketEntry);
					if (GoldenNames.Contains(ticketEntry.PlayerName))
					{
						Vector4 vector = new Vector4(1f, 0.85f, 0f, 1f);
						((ImU8String)(ref val))._002Ector(2, 1);
						((ImU8String)(ref val)).AppendLiteral("\ue06c ");
						((ImU8String)(ref val)).AppendFormatted<string>(text);
						ImGui.TextColored(ref vector, val);
					}
					else
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit(text));
					}
					ImGui.SameLine();
					((ImU8String)(ref val))._002Ector(12, 1);
					((ImU8String)(ref val)).AppendLiteral("\ue05e Edit##edit");
					((ImU8String)(ref val)).AppendFormatted<int>(i);
					if (ImGui.SmallButton(val))
					{
						selectedIndex = i;
						editName = ticketEntry.PlayerName;
						editBase = ticketEntry.BaseTickets;
						editBonus = ticketEntry.BonusTickets;
						editVIP = ticketEntry.FreeTickets;
					}
				}
				ImGui.EndChild();
				ImGui.Separator();
				ImGui.TextUnformatted(ImU8String.op_Implicit(" Combined Summary (by Player)"));
				ImGui.SameLine();
				ImGui.Checkbox(ImU8String.op_Implicit("Show Server Name (Summary)"), ref showWorldSummary);
				ImGui.BeginChild(ImU8String.op_Implicit("CombinedSummaryScroll"), new Vector2(0f, 180f), true, (ImGuiWindowFlags)0);
				var list = (from g in plugin.Entries.GroupBy<TicketEntry, string>((TicketEntry e) => BaseName(e.PlayerName), StringComparer.OrdinalIgnoreCase)
					select new
					{
						Name = (from x in g
							select x.PlayerName into n
							orderby n.Contains('@') descending
							select n).First(),
						Base = g.Sum((TicketEntry x) => x.BaseTickets),
						Bonus = g.Sum((TicketEntry x) => x.BonusTickets),
						Free = g.Sum((TicketEntry x) => x.FreeTickets),
						Total = g.Sum((TicketEntry x) => x.TotalTickets)
					} into x
					orderby x.Total descending, x.Name
					select x).ToList();
				foreach (var item in list)
				{
					string value = FormatDisplayName(item.Name, showWorldSummary);
					List<string> list2 = new List<string> { item.Base.ToString() };
					if (item.Bonus > 0)
					{
						list2.Add($"+ {item.Bonus}");
					}
					if (item.Free > 0)
					{
						list2.Add($"+ {item.Free} VIP");
					}
					string text2 = $"{value} = {string.Join(" ", list2)} = {item.Total} tickets";
					if (GoldenNames.Contains(item.Name))
					{
						Vector4 vector = new Vector4(1f, 0.85f, 0f, 1f);
						val = new ImU8String(2, 1);
						((ImU8String)(ref val)).AppendLiteral("\ue06c ");
						((ImU8String)(ref val)).AppendFormatted<string>(text2);
						ImGui.TextColored(ref vector, val);
					}
					else
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit(text2));
					}
				}
				ImGui.EndChild();
				if (ImGui.Button(ImU8String.op_Implicit("\ue0be Copy Summary to Clipboard"), default(Vector2)))
				{
					IEnumerable<string> values = list.Select(row =>
					{
						string value2 = FormatDisplayName(row.Name, showWorldSummary);
						List<string> list6 = new List<string> { row.Base.ToString() };
						if (row.Bonus > 0)
						{
							list6.Add($"+ {row.Bonus}");
						}
						if (row.Free > 0)
						{
							list6.Add($"+ {row.Free} VIP");
						}
						return $"{value2} = {string.Join(" ", list6)} = {row.Total} tickets";
					});
					ImGui.SetClipboardText(ImU8String.op_Implicit(string.Join("\n", values)));
				}
				if (selectedIndex >= 0 && selectedIndex < plugin.Entries.Count)
				{
					ImGui.Separator();
					((ImU8String)(ref val))._002Ector(16, 1);
					((ImU8String)(ref val)).AppendLiteral("Editing ticket #");
					((ImU8String)(ref val)).AppendFormatted<int>(selectedIndex + 1);
					ImGui.Text(val);
					ImGui.InputText(ImU8String.op_Implicit("Player Name"), ref editName, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					ImU8String val2 = ImU8String.op_Implicit("Base Tickets");
					ref int reference = ref editBase;
					val = default(ImU8String);
					ImGui.InputInt(val2, ref reference, 0, 0, val, (ImGuiInputTextFlags)0);
					editBase = Math.Max(0, editBase);
					ImU8String val3 = ImU8String.op_Implicit("Bonus Tickets");
					ref int reference2 = ref editBonus;
					val = default(ImU8String);
					ImGui.InputInt(val3, ref reference2, 0, 0, val, (ImGuiInputTextFlags)0);
					editBonus = Math.Max(0, editBonus);
					ImU8String val4 = ImU8String.op_Implicit("VIP Free Tickets");
					ref int reference3 = ref editVIP;
					val = default(ImU8String);
					ImGui.InputInt(val4, ref reference3, 0, 0, val, (ImGuiInputTextFlags)0);
					editVIP = Math.Max(0, editVIP);
					if (ImGui.Button(ImU8String.op_Implicit("\ue06f Save Changes"), default(Vector2)))
					{
						TicketEntry ticketEntry2 = plugin.Entries[selectedIndex];
						ticketEntry2.PlayerName = editName.Trim();
						ticketEntry2.BaseTickets = editBase;
						ticketEntry2.BonusTickets = editBonus;
						ticketEntry2.FreeTickets = editVIP;
						plugin.SaveEntries();
						selectedIndex = -1;
					}
					ImGui.SameLine();
					if (ImGui.Button(ImU8String.op_Implicit("\ue0bf Cancel"), default(Vector2)))
					{
						selectedIndex = -1;
					}
					ImGui.SameLine();
					if (ImGui.Button(ImU8String.op_Implicit("\ue05f Delete Entry"), default(Vector2)))
					{
						plugin.Entries.RemoveAt(selectedIndex);
						plugin.SaveEntries();
						selectedIndex = -1;
					}
				}
				ImGui.EndTabItem();
			}
			if (ImGui.BeginTabItem(ImU8String.op_Implicit("\ue048 Discord Preview"), (ImGuiTabItemFlags)0))
			{
				ImGui.InputText(ImU8String.op_Implicit("Discord Header"), ref discordHeader, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				ImGui.Separator();
				ImGui.TextUnformatted(ImU8String.op_Implicit(discordHeader));
				ImGui.BeginChild(ImU8String.op_Implicit("DiscordTicketScroll"), new Vector2(0f, 250f), true, (ImGuiWindowFlags)0);
				int num = 1;
				foreach (TicketEntry entry in plugin.Entries)
				{
					for (int num2 = 0; num2 < entry.TotalTickets; num2++)
					{
						val = new ImU8String(2, 2);
						((ImU8String)(ref val)).AppendFormatted<int>(num++, 3);
						((ImU8String)(ref val)).AppendLiteral("  ");
						((ImU8String)(ref val)).AppendFormatted<string>(entry.PlayerName);
						ImGui.TextUnformatted(val);
					}
				}
				ImGui.EndChild();
				ImGui.Separator();
				if (ImGui.Button(ImU8String.op_Implicit("\ue0be Copy This View to Clipboard"), default(Vector2)))
				{
					List<string> list3 = new List<string> { discordHeader };
					int num3 = 1;
					foreach (TicketEntry entry2 in plugin.Entries)
					{
						for (int num4 = 0; num4 < entry2.TotalTickets; num4++)
						{
							list3.Add($"{num3++,3}  {entry2.PlayerName}");
						}
					}
					ImGui.SetClipboardText(ImU8String.op_Implicit(string.Join("\n", list3)));
				}
				if (ImGui.Button(ImU8String.op_Implicit("\ue0be Preview Discord Chunks"), default(Vector2)))
				{
					List<string> list4 = new List<string> { discordHeader };
					int num5 = 1;
					foreach (TicketEntry entry3 in plugin.Entries)
					{
						for (int num6 = 0; num6 < entry3.TotalTickets; num6++)
						{
							list4.Add($"{num5++,3}  {entry3.PlayerName}");
						}
					}
					string fullMessage = string.Join("\n", list4);
					discordChunks = SplitIntoChunks(fullMessage);
				}
				for (int num7 = 0; num7 < discordChunks.Count; num7++)
				{
					((ImU8String)(ref val))._002Ector(13, 1);
					((ImU8String)(ref val)).AppendLiteral("\ue0be Copy Chunk ");
					((ImU8String)(ref val)).AppendFormatted<int>(num7 + 1);
					if (ImGui.Button(val, default(Vector2)))
					{
						ImGui.SetClipboardText(ImU8String.op_Implicit(discordChunks[num7]));
					}
					ImGui.SameLine();
					((ImU8String)(ref val))._002Ector(15, 2);
					((ImU8String)(ref val)).AppendLiteral("Chunk ");
					((ImU8String)(ref val)).AppendFormatted<int>(num7 + 1);
					((ImU8String)(ref val)).AppendLiteral(" (");
					((ImU8String)(ref val)).AppendFormatted<int>(discordChunks[num7].Length);
					((ImU8String)(ref val)).AppendLiteral(" chars)");
					ImGui.Text(val);
					string text3 = discordChunks[num7];
					((ImU8String)(ref val))._002Ector(7, 1);
					((ImU8String)(ref val)).AppendLiteral("##chunk");
					((ImU8String)(ref val)).AppendFormatted<int>(num7);
					ImGui.InputTextMultiline(val, ref text3, 4000, new Vector2(350f, 120f), (ImGuiInputTextFlags)16384, (ImGuiInputTextCallbackDelegate)null);
					ImGui.Separator();
				}
				ImGui.EndTabItem();
			}
			if (ImGui.BeginTabItem(ImU8String.op_Implicit("\ue03a Wheel Export"), (ImGuiTabItemFlags)0))
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit("\ue03a Copy this list to paste into a wheel picker site:"));
				ImGui.TextUnformatted(ImU8String.op_Implicit("Each line includes the ticket number."));
				ImGui.Separator();
				List<string> list5 = new List<string>();
				int num8 = 1;
				foreach (TicketEntry entry4 in plugin.Entries)
				{
					for (int num9 = 0; num9 < entry4.TotalTickets; num9++)
					{
						list5.Add($"{num8++}. {entry4.PlayerName}");
					}
				}
				string text4 = string.Join("\n", list5);
				ImGui.InputTextMultiline(ImU8String.op_Implicit("##wheelText"), ref text4, 10000, new Vector2(-1f, 250f), (ImGuiInputTextFlags)16384, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.Button(ImU8String.op_Implicit("\ue040 Copy to Clipboard"), default(Vector2)))
				{
					ImGui.SetClipboardText(ImU8String.op_Implicit(text4));
				}
				ImGui.EndTabItem();
			}
			ImGui.EndTabBar();
		}
		static string BaseName(string s)
		{
			return s.Split('@')[0].Trim();
		}
		static string ComposeEntryLine(string displayName, TicketEntry e)
		{
			List<string> list6 = new List<string> { e.BaseTickets.ToString() };
			if (e.BonusTickets > 0)
			{
				list6.Add($"+ {e.BonusTickets}");
			}
			if (e.FreeTickets > 0)
			{
				list6.Add($"+ {e.FreeTickets} VIP");
			}
			return $"{displayName} = {string.Join(" ", list6)} = {e.TotalTickets} tickets";
		}
	}
}
