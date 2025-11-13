using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace EasyRaffle.UI;

public static class EasyRaffleTheme
{
	private static int _styleColorCount;

	private static int _styleVarCount;

	public static void Push()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ImGuiStylePtr style = ImGui.GetStyle();
		_ = ((ImGuiStylePtr)(ref style)).Colors;
		Vector4 vector = new Vector4(0f, 1f, 1f, 1f);
		Vector4 vector2 = new Vector4(0.06f, 0.06f, 0.08f, 1f);
		Vector4 vector3 = new Vector4(0.1f, 0.1f, 0.12f, 1f);
		ImGui.PushStyleColor((ImGuiCol)2, vector2);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)3, vector2);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)4, vector2);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)24, vector * new Vector4(1f, 1f, 1f, 0.4f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)25, vector * new Vector4(1f, 1f, 1f, 0.7f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)26, vector);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)21, vector * new Vector4(1f, 1f, 1f, 0.4f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)22, vector * new Vector4(1f, 1f, 1f, 0.7f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)23, vector);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)7, vector3);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)8, vector * new Vector4(1f, 1f, 1f, 0.25f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)9, vector * new Vector4(1f, 1f, 1f, 0.4f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)33, vector3);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)34, vector * new Vector4(1f, 1f, 1f, 0.6f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)35, vector * new Vector4(1f, 1f, 1f, 0.8f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)10, vector3);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)11, vector * new Vector4(1f, 1f, 1f, 0.6f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)19, vector * new Vector4(1f, 1f, 1f, 0.7f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)20, vector);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)18, vector);
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)27, vector * new Vector4(1f, 1f, 1f, 0.3f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)28, vector * new Vector4(1f, 1f, 1f, 0.7f));
		_styleColorCount++;
		ImGui.PushStyleColor((ImGuiCol)29, vector);
		_styleColorCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)2, 6f);
		_styleVarCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)6, 6f);
		_styleVarCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)11, 5f);
		_styleVarCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)20, 5f);
		_styleVarCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)21, 5f);
		_styleVarCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)12, 1f);
		_styleVarCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)3, 1f);
		_styleVarCount++;
		ImGui.PushStyleVar((ImGuiStyleVar)9, 1f);
		_styleVarCount++;
	}

	public static void Pop()
	{
		ImGui.PopStyleColor(_styleColorCount);
		ImGui.PopStyleVar(_styleVarCount);
		_styleColorCount = 0;
		_styleVarCount = 0;
	}
}
