using System;
using System.IO;
using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace EasyRaffle.UI;

public class FilePicker
{
	private bool isOpen;

	private string directoryPath = "";

	private string selectedFile = "";

	private Action<string>? onFileSelected;

	public void Open(string title, string initialPath, Action<string> onSelect)
	{
		isOpen = true;
		directoryPath = initialPath;
		selectedFile = "";
		onFileSelected = onSelect;
	}

	public void Draw(string windowTitle = "Choose a CSV file")
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		if (!isOpen)
		{
			return;
		}
		ImGui.SetNextWindowSize(new Vector2(600f, 400f), (ImGuiCond)4);
		if (ImGui.Begin(ImU8String.op_Implicit(windowTitle), ref isOpen, (ImGuiWindowFlags)0))
		{
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(19, 1);
			((ImU8String)(ref val)).AppendLiteral("Current Directory: ");
			((ImU8String)(ref val)).AppendFormatted<string>(directoryPath);
			ImGui.Text(val);
			if (ImGui.Button(ImU8String.op_Implicit("⬆ Up"), default(Vector2)))
			{
				directoryPath = Directory.GetParent(directoryPath)?.FullName ?? directoryPath;
			}
			ImGui.Separator();
			string[] directories = Directory.GetDirectories(directoryPath);
			foreach (string path in directories)
			{
				if (ImGui.Selectable(ImU8String.op_Implicit("[DIR] " + Path.GetFileName(path)), false, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					directoryPath = path;
				}
			}
			directories = Directory.GetFiles(directoryPath, "*.csv");
			foreach (string path2 in directories)
			{
				if (ImGui.Selectable(ImU8String.op_Implicit(Path.GetFileName(path2)), false, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					selectedFile = path2;
				}
			}
			ImGui.Separator();
			((ImU8String)(ref val))._002Ector(10, 1);
			((ImU8String)(ref val)).AppendLiteral("Selected: ");
			((ImU8String)(ref val)).AppendFormatted<string>(selectedFile);
			ImGui.Text(val);
			if (ImGui.Button(ImU8String.op_Implicit("✅ Load Selected File"), default(Vector2)) && !string.IsNullOrEmpty(selectedFile))
			{
				onFileSelected?.Invoke(selectedFile);
				isOpen = false;
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit("❌ Cancel"), default(Vector2)))
			{
				isOpen = false;
			}
		}
		ImGui.End();
	}
}
