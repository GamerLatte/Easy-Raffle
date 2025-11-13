namespace EasyRaffle.Windows;

public class VIPConfig
{
	public string VIPName { get; set; } = string.Empty;

	public int FreeTickets { get; set; }

	public uint VIPColor { get; set; }

	public bool AllowExtraTickets { get; set; }

	public bool AllowBogoOverride { get; set; }
}
