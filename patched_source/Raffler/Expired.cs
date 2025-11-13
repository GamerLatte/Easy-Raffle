using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace Raffler;

public class Expired
{
	public bool ShouldClose { get; private set; }

	public void Draw()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (!ShouldClose)
		{
			ImGui.SetNextWindowSize(new Vector2(400f, 180f), (ImGuiCond)8);
			ImGui.Begin(ImU8String.op_Implicit("Raffler - Plugin Expired"), (ImGuiWindowFlags)98);
			ImGui.TextWrapped(ImU8String.op_Implicit("\ud83c\udf89 Thank you for using Raffler!"));
			ImGui.Spacing();
			ImGui.TextWrapped(ImU8String.op_Implicit("\ud83d\udd70\ufe0f The plugin usage period has ended as of June 20, 2025."));
			ImGui.Spacing();
			ImGui.TextWrapped(ImU8String.op_Implicit("\ud83d\udc8c We appreciate your support - please don't hesitate to share feedback or thoughts!"));
			ImGui.Spacing();
			if (ImGui.Button(ImU8String.op_Implicit("❌ Close Plugin"), default(Vector2)))
			{
				ShouldClose = true;
			}
			ImGui.End();
		}
	}
}
