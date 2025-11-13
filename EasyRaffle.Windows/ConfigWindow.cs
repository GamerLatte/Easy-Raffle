using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using EasyRaffle.UI;

namespace EasyRaffle.Windows;

public class ConfigWindow : Window, IDisposable
{
	private Configuration Configuration;

	public ConfigWindow(Plugin plugin)
		: base("Easy Raffle Config###With a constant ID")
	{
		((Window)this).Flags = (ImGuiWindowFlags)58;
		((Window)this).Size = new Vector2(232f, 110f);
		((Window)this).SizeCondition = (ImGuiCond)1;
		Configuration = plugin.Configuration;
		if (Configuration.Version == 0)
		{
			Configuration.ShowDebugOption = true;
			Configuration.StartingPotMillions = 10;
		}
	}

	public void Dispose()
	{
	}

	public override void PreDraw()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (Configuration.IsConfigWindowMovable)
		{
			((Window)this).Flags = (ImGuiWindowFlags)(((Window)this).Flags & -5);
		}
		else
		{
			((Window)this).Flags = (ImGuiWindowFlags)(((Window)this).Flags | 4);
		}
	}

	public override void Draw()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		bool showDebugOption = Configuration.ShowDebugOption;
		if (ImGui.Checkbox(ImU8String.op_Implicit("Enable Debug Mode"), ref showDebugOption))
		{
			Configuration.ShowDebugOption = showDebugOption;
			Configuration.Save();
		}
		bool isConfigWindowMovable = Configuration.IsConfigWindowMovable;
		if (ImGui.Checkbox(ImU8String.op_Implicit("Movable Config Window"), ref isConfigWindowMovable))
		{
			Configuration.IsConfigWindowMovable = isConfigWindowMovable;
			Configuration.Save();
		}
		int startingPotMillions = Configuration.StartingPotMillions;
		if (ImGui.InputInt(ImU8String.op_Implicit("Starting Pot (mil)"), ref startingPotMillions, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0))
		{
			Configuration.StartingPotMillions = startingPotMillions;
			Configuration.Save();
		}
		EasyRaffleTheme.Pop();
	}
}
