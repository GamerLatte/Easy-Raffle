using System;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Raffler.Data;

namespace Raffler.Windows;

public class PaySummaryWindow : Window
{
	private readonly Plugin plugin;

	private bool receivedStartingPot;

	private float startingGilReceived;

	public PaySummaryWindow(Plugin plugin)
		: base("Raffler - Pay Summary", (ImGuiWindowFlags)64, false)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		this.plugin = plugin;
		WindowSizeConstraints value = default(WindowSizeConstraints);
		((WindowSizeConstraints)(ref value))._002Ector();
		((WindowSizeConstraints)(ref value)).MinimumSize = new Vector2(420f, 200f);
		((WindowSizeConstraints)(ref value)).MaximumSize = new Vector2(900f, 900f);
		((Window)this).SizeConstraints = value;
	}

	public override void Draw()
	{
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
		Configuration configuration = plugin.Configuration;
		int num = plugin.Entries.Sum((TicketEntry e) => e.BaseTickets);
		int num2 = plugin.Entries.Sum((TicketEntry e) => e.BonusTickets);
		int num3 = plugin.Entries.Sum((TicketEntry e) => e.FreeTickets);
		bool countBonusTicketsTowardPot = configuration.CountBonusTicketsTowardPot;
		bool countVipTicketsTowardPot = configuration.CountVipTicketsTowardPot;
		int value = num + num2 + num3;
		int num4 = num + (countBonusTicketsTowardPot ? num2 : 0) + (countVipTicketsTowardPot ? num3 : 0);
		float num5 = (float)num * configuration.TicketCost;
		float num6 = (float)num4 * configuration.TicketCost;
		float num7 = (float)(configuration.ActiveSessionStartingPotMillions ?? configuration.StartingPotMillions) * 1000000f;
		float num8 = num7 + num6;
		float num9 = num8 / 2f;
		float num10 = (receivedStartingPot ? MathF.Max(0f, startingGilReceived) : 0f);
		float num11 = num10 + num5 - num9;
		Vector4 vector = new Vector4(1f, 0.95f, 0.35f, 1f);
		Vector4 value2 = new Vector4(0.85f, 0.9f, 1f, 1f);
		Vector4 value3 = new Vector4(0.5f, 1f, 0.5f, 1f);
		Vector4 vector2 = new Vector4(1f, 0.4f, 0.4f, 1f);
		ImGui.TextColored(ref vector, ImU8String.op_Implicit("Pay Summary"));
		ImGui.Separator();
		if (ImGui.BeginTable(ImU8String.op_Implicit("##ps_overview"), 2, (ImGuiTableFlags)24576, default(Vector2), 0f))
		{
			ImGui.TableSetupColumn(ImU8String.op_Implicit("L"), (ImGuiTableColumnFlags)4, 0.6f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("R"), (ImGuiTableColumnFlags)4, 0.4f, 0u);
			RowWithTooltip("Advertised start", GilF(num7) + " (" + MilF(num7) + ")", "This is the banner starting pot before any tickets are sold.", value2);
			RowWithTooltip("Count BOGO toward pot", countBonusTicketsTowardPot ? "Yes" : "No", "Controls whether free BOGO tickets add gil to the pot.");
			RowWithTooltip("Count VIP toward pot", countVipTicketsTowardPot ? "Yes" : "No", "Controls whether VIP free tickets add gil to the pot.");
			RowWithTooltip("Tickets issued (incl. freebies)", value.ToString("N0"), "Total tickets handed out, including freebies.");
			RowWithTooltip("Tickets counted toward pot", num4.ToString("N0"), "Tickets that actually contribute gil to the pot based on the above toggles.");
			RowWithTooltip("Advertised pot (now)", GilF(num8) + " (" + MilF(num8) + ")", "Current banner pot: advertised start plus the gil from counted tickets.", vector);
			ImGui.EndTable();
		}
		ImGui.Separator();
		ImGui.TextColored(ref value2, ImU8String.op_Implicit("Starting gil handling"));
		ImGui.Checkbox(ImU8String.op_Implicit("Did you receive or start with any of the pot?"), ref receivedStartingPot);
		if (ImGui.IsItemHovered())
		{
			ImGui.BeginTooltip();
			ImGui.TextUnformatted(ImU8String.op_Implicit("The starting pot is sometimes held by the venue owner until the raffle is pulled"));
			ImGui.EndTooltip();
		}
		if (receivedStartingPot)
		{
			ImGui.SameLine();
			ImGui.SetNextItemWidth(160f);
			float y = startingGilReceived / 1000000f;
			ImGui.InputFloat(ImU8String.op_Implicit("How much (M)"), ref y, 0.1f, 1f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
			y = MathF.Max(0f, y);
			startingGilReceived = y * 1000000f;
		}
		ImGui.Separator();
		ImGui.TextColored(ref value2, ImU8String.op_Implicit("Tickets"));
		if (ImGui.BeginTable(ImU8String.op_Implicit("##ps_tickets"), 3, (ImGuiTableFlags)24576, default(Vector2), 0f))
		{
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Type"), (ImGuiTableColumnFlags)4, 0.6f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Count"), (ImGuiTableColumnFlags)4, 0.4f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Counted?"), (ImGuiTableColumnFlags)4, 0.4f, 0u);
			TRow("Paid", num, counted: true, "Tickets purchased with gil always count toward the pot.");
			TRow("BOGO", num2, countBonusTicketsTowardPot, "Free tickets earned from buy-one-get-one promotions.");
			TRow("VIP free", num3, countVipTicketsTowardPot, "Complimentary tickets granted from VIP perks.");
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit("Total issued"));
			ImGui.TableNextColumn();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(value.ToString("N0")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(""));
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit("Total counted toward pot"));
			ImGui.TableNextColumn();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(num4.ToString("N0")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(""));
			ImGui.EndTable();
		}
		ImGui.Separator();
		ImGui.TextColored(ref value2, ImU8String.op_Implicit("Breakdown"));
		if (ImGui.BeginTable(ImU8String.op_Implicit("##ps_breakdown"), 3, (ImGuiTableFlags)24576, default(Vector2), 0f))
		{
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Line"), (ImGuiTableColumnFlags)4, 0.5f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Formula"), (ImGuiTableColumnFlags)4, 0.9f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("Result"), (ImGuiTableColumnFlags)4, 0.6f, 0u);
			Row("Gil collected (paid tickets)", $"{num:N0} × {configuration.TicketCost:N0}", GilF(num5) + " (" + MilF(num5) + ")", "Total gil received from selling paid tickets only.");
			Row("Winner payout (50%)", "Advertised pot / 2", GilF(num9) + " (" + MilF(num9) + ")", "Half of the advertised pot goes to the winner.", value3);
			Vector4 value4 = ((num11 >= 0f) ? vector : vector2);
			Row("Net house balance", $"{GilF(num10)} + {GilF(num5)} - {GilF(num9)}", GilF(num11) + " (" + MilF(num11) + ")", (num11 >= 0f) ? "Positive values mean the venue keeps gil after paying the winner." : "Negative values mean the venue must contribute additional gil to pay the winner.", value4);
			ImGui.EndTable();
		}
		ImGui.Separator();
		if (ImGui.Button(ImU8String.op_Implicit("Copy summary to clipboard"), default(Vector2)))
		{
			ImGui.SetClipboardText(ImU8String.op_Implicit(string.Join("\n", $"Advertised start: {GilF(num7)} ({MilF(num7)})", $"Tickets: paid {num:N0}, bogo {num2:N0} ({(countBonusTicketsTowardPot ? "counted" : "not counted")}), vip {num3:N0} ({(countVipTicketsTowardPot ? "counted" : "not counted")}), issued {value:N0}, counted {num4:N0}", $"Advertised pot now: {GilF(num8)} ({MilF(num8)})", $"Gil collected (paid tickets): {GilF(num5)} ({MilF(num5)})", $"Winner (50% of advertised): {GilF(num9)} ({MilF(num9)})", $"Net house balance: {GilF(num11)} ({MilF(num11)})")));
		}
		static string GilF(float g)
		{
			return g.ToString("N0") + " gil";
		}
		static string MilF(float g)
		{
			return (g / 1000000f).ToString("N1") + "m";
		}
		static void Row(string name, string formula, string result, string tooltip, Vector4? color = null)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(name));
			AddTooltip(tooltip);
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(formula));
			AddTooltip(tooltip);
			ImGui.TableNextColumn();
			if (color.HasValue)
			{
				Vector4 value5 = color.Value;
				ImGui.TextColored(ref value5, ImU8String.op_Implicit(result));
			}
			else
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(result));
			}
			AddTooltip(tooltip);
		}
		static void TRow(string name, int count, bool counted, string? tooltip)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(name));
			AddTooltip(tooltip);
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(count.ToString("N0")));
			AddTooltip(tooltip);
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(counted ? "Yes" : "No"));
			AddTooltip(counted ? "These tickets add gil to the pot." : "These tickets do not add gil to the pot.");
		}
	}

	private static void AddTooltip(string? text)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!string.IsNullOrEmpty(text) && ImGui.IsItemHovered((ImGuiHoveredFlags)128))
		{
			ImGui.BeginTooltip();
			ImGui.TextUnformatted(ImU8String.op_Implicit(text));
			ImGui.EndTooltip();
		}
	}

	private static void RowWithTooltip(string label, string value, string tooltip, Vector4? valueColor = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(label));
		AddTooltip(tooltip);
		ImGui.TableNextColumn();
		if (valueColor.HasValue)
		{
			Vector4 value2 = valueColor.Value;
			ImGui.TextColored(ref value2, ImU8String.op_Implicit(value));
		}
		else
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(value));
		}
		AddTooltip(tooltip);
	}
}
