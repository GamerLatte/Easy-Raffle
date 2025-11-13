public static class ImGuiFileDialog
{
	private static string _path = "";

	private static bool _display = false;

	private static string _key = "";

	public static void OpenDialog(string key, string title, string ext, string path)
	{
		_display = true;
		_key = key;
	}

	public static bool Display(string key)
	{
		if (_display)
		{
			return _key == key;
		}
		return false;
	}

	public static bool IsOk()
	{
		_display = false;
		return true;
	}

	public static string GetFilePathName()
	{
		return "C:/Path/To/Your/raffle_tickets.csv";
	}

	public static void Close()
	{
		_display = false;
	}
}
