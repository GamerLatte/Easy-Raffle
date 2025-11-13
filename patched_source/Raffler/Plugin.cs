using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ECommons;
using Raffler.Data;
using Raffler.Windows;

namespace Raffler;

public sealed class Plugin : IDalamudPlugin, IDisposable
{
	public sealed class VipTracker
	{
		private readonly Dictionary<string, int> usedFreebies = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

		public int GetUsed(string name)
		{
			if (!usedFreebies.TryGetValue(name, out var value))
			{
				return 0;
			}
			return value;
		}

		public void AddUsed(string name, int add)
		{
			if (add > 0)
			{
				usedFreebies[name] = GetUsed(name) + add;
			}
		}

		public void Reset()
		{
			usedFreebies.Clear();
		}
	}

	private const string CommandName = "/raffler";

	public readonly WindowSystem WindowSystem = new WindowSystem("Raffler");

	private static readonly JsonSerializerOptions TicketJsonOptions = new JsonSerializerOptions
	{
		WriteIndented = true,
		PropertyNameCaseInsensitive = true
	};

	public VipRuntime Vip { get; } = new VipRuntime();

	public SessionStore Session { get; private set; }

	[PluginService]
	internal static IDalamudPluginInterface PluginInterface { get; private set; } = null;

	[PluginService]
	internal static ITextureProvider TextureProvider { get; private set; } = null;

	[PluginService]
	internal static ICommandManager CommandManager { get; private set; } = null;

	[PluginService]
	internal static IClientState ClientState { get; private set; } = null;

	[PluginService]
	internal static IObjectTable ObjectTable { get; private set; } = null;

	[PluginService]
	internal static IDataManager DataManager { get; private set; } = null;

	[PluginService]
	internal static IPluginLog Log { get; private set; } = null;

	[PluginService]
	internal static ITargetManager TargetManager { get; private set; } = null;

	[PluginService]
	internal static IChatGui ChatGui { get; private set; } = null;

	[PluginService]
	internal static IFramework Framework { get; private set; } = null;

	[PluginService]
	internal static IDalamudPluginInterface DalamudInterface { get; private set; } = null;

	public DateTime SessionStartTime { get; set; } = DateTime.Now;

	public Configuration Configuration { get; init; }

	private ConfigWindow ConfigWindow { get; init; }

	private MainWindow MainWindow { get; init; }

	private TicketListWindow TicketListWindow { get; init; }

	private PaySummaryWindow PaySummaryWindow { get; init; }

	public AuthUsers AuthUsers { get; private set; } = new AuthUsers();

	public bool IsAuthorizedUser { get; private set; }

	public List<TicketEntry> Entries { get; private set; } = new List<TicketEntry>();

	public int BonusTicketsRemaining { get; set; }

	public string LastTellTarget { get; private set; } = "";

	public CsvImporter CsvImporter { get; private set; }

	private RafflerLiteWindow RafflerLiteWindow { get; init; }

	public AuthorizationManager AuthManager { get; private set; }

	public Dictionary<int, string> TicketMessageMap { get; private set; } = new Dictionary<int, string>();

	private string TicketSavePath => Path.Combine(PluginInterface.ConfigDirectory.FullName, "raffle_entries.json");

	public Plugin()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		ECommonsMain.Init(PluginInterface, (IDalamudPlugin)(object)this, Array.Empty<Module>());
		AuthManager = new AuthorizationManager(Framework, ClientState, Log, ChatGui);
		ChatGui.ChatMessage += new OnMessageDelegate(OnChatMessage);
		CsvImporter = new CsvImporter(this);
		Configuration = (PluginInterface.GetPluginConfig() as Configuration) ?? new Configuration();
		string raffleImgArg = Path.Combine(PluginInterface.AssemblyLocation.Directory.FullName, "raffler.png");
		ConfigWindow = new ConfigWindow(this);
		TicketListWindow = new TicketListWindow(this);
		MainWindow = new MainWindow(this, raffleImgArg, TicketListWindow, null);
		RafflerLiteWindow = new RafflerLiteWindow(this, MainWindow, TicketListWindow);
		PaySummaryWindow = new PaySummaryWindow(this);
		WindowSystem.AddWindow((Window)(object)ConfigWindow);
		WindowSystem.AddWindow((Window)(object)MainWindow);
		WindowSystem.AddWindow((Window)(object)TicketListWindow);
		WindowSystem.AddWindow((Window)(object)RafflerLiteWindow);
		WindowSystem.AddWindow((Window)(object)PaySummaryWindow);
		string fullName = PluginInterface.ConfigDirectory.FullName;
		Session = new SessionStore(fullName);
		bool flag = false;
		if (Session.HadCrashLastRun())
		{
			flag = TryRestoreFromSession();
		}
		CommandManager.AddHandler("/raffler", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = "Open Raffler UI."
		});
		CommandManager.AddHandler("/rafflerlite", new CommandInfo((HandlerDelegate)delegate
		{
			((Window)RafflerLiteWindow).Toggle();
		})
		{
			HelpMessage = "Open Raffler Lite"
		});
		PluginInterface.UiBuilder.Draw += DrawUI;
		PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;
		PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
		PluginInterface.UiBuilder.OpenMainUi += TicketListUI;
		if (!flag)
		{
			if (LoadEntries())
			{
				Log.Information($"↻ Loaded {Entries.Count} ticket entries from disk.", Array.Empty<object>());
			}
			SyncSession();
		}
		Log.Information($"=== Raffler plugin {PluginInterface.Manifest} loaded! ===", Array.Empty<object>());
	}

	private void OnChatMessage(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		if ((int)type == 13)
		{
			LastTellTarget = sender.TextValue;
			SyncSession();
		}
		else if ((int)type == 12)
		{
			LastTellTarget = sender.TextValue;
			SyncSession();
		}
	}

	public void Dispose()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		ECommonsMain.Dispose();
		WindowSystem.RemoveAllWindows();
		ConfigWindow.Dispose();
		MainWindow.Dispose();
		TicketListWindow.Dispose();
		ChatGui.ChatMessage -= new OnMessageDelegate(OnChatMessage);
		CommandManager.RemoveHandler("/raffler");
		AuthManager.Dispose();
		try
		{
			Session?.MarkCleanAndSave();
		}
		catch
		{
		}
		Session?.Dispose();
	}

	public void SaveEntries()
	{
		try
		{
			string contents = JsonSerializer.Serialize(Entries, TicketJsonOptions);
			File.WriteAllText(TicketSavePath, contents);
		}
		catch (Exception ex)
		{
			Log.Error("Failed to save ticket entries: " + ex.Message, Array.Empty<object>());
		}
		finally
		{
			SyncSession();
		}
	}

	public bool LoadEntries()
	{
		if (!File.Exists(TicketSavePath))
		{
			Entries = new List<TicketEntry>();
			return false;
		}
		try
		{
			List<TicketEntry> list = JsonSerializer.Deserialize<List<TicketEntry>>(File.ReadAllText(TicketSavePath), TicketJsonOptions);
			Entries = ((list != null) ? new List<TicketEntry>(list) : new List<TicketEntry>());
			return Entries.Count > 0;
		}
		catch (Exception ex)
		{
			Log.Error("Failed to load ticket entries: " + ex.Message, Array.Empty<object>());
			Entries = new List<TicketEntry>();
			return false;
		}
	}

	private void OnCommand(string command, string args)
	{
		ToggleMainUI();
	}

	private void DrawUI()
	{
		WindowSystem.Draw();
	}

	public void ToggleConfigUI()
	{
		((Window)ConfigWindow).Toggle();
	}

	public void TogglePaySummaryUI()
	{
		((Window)PaySummaryWindow).Toggle();
	}

	public void ToggleMainUI()
	{
		if (!AuthManager.Initialized)
		{
			ChatGui.PrintError("⏳ Authorization is still loading. Try again in a few seconds.", (string)null, (ushort?)null);
		}
		else if (!AuthManager.IsAuthorizedUser)
		{
			ChatGui.PrintError("⛔ You are not authorized to open Raffler.", (string)null, (ushort?)null);
		}
		else
		{
			((Window)MainWindow).Toggle();
		}
	}

	public void TicketListUI()
	{
		((Window)TicketListWindow).Toggle();
	}

	public void SyncSession()
	{
		if (Session != null)
		{
			Session.Update(delegate(SessionState s)
			{
				s.ActiveStartMil = Configuration.ActiveSessionStartingPotMillions.GetValueOrDefault();
				s.BonusTicketsRemaining = BonusTicketsRemaining;
				s.Entries = new List<TicketEntry>(Entries);
				s.VenueMacro = Configuration.VenueMacro ?? string.Empty;
				s.EnableTicketRangeTells = Configuration.EnableTicketRangeTells;
				s.TicketRangeTemplate = Configuration.TicketRangeTemplate ?? string.Empty;
				s.DiscordWebhookUrl = Configuration.DiscordWebhookUrl ?? string.Empty;
				s.LastTellTarget = LastTellTarget;
				s.VipUsedFreebies = new Dictionary<string, int>(Vip.Snapshot(), StringComparer.OrdinalIgnoreCase);
			});
		}
	}

	private bool TryRestoreFromSession()
	{
		try
		{
			SessionState state = Session.State;
			BonusTicketsRemaining = Math.Max(0, state.BonusTicketsRemaining);
			List<TicketEntry> list = (Entries = ((state.Entries != null) ? (from e in state.Entries
				where e != null
				select new TicketEntry
				{
					PlayerName = e.PlayerName,
					BaseTickets = e.BaseTickets,
					BonusTickets = e.BonusTickets,
					FreeTickets = e.FreeTickets
				}).ToList() : new List<TicketEntry>()));
			Vip.LoadFrom(state.VipUsedFreebies);
			bool flag = list.Count > 0;
			int num = state.VipUsedFreebies?.Count ?? 0;
			bool result = flag || state.BonusTicketsRemaining > 0 || num > 0;
			Configuration.ActiveSessionStartingPotMillions = (flag ? new int?(state.ActiveStartMil) : ((int?)null));
			Configuration.VenueMacro = state.VenueMacro ?? Configuration.VenueMacro;
			Configuration.EnableTicketRangeTells = state.EnableTicketRangeTells;
			Configuration.DiscordWebhookUrl = state.DiscordWebhookUrl ?? Configuration.DiscordWebhookUrl;
			if (!string.IsNullOrWhiteSpace(state.TicketRangeTemplate))
			{
				Configuration.TicketRangeTemplate = state.TicketRangeTemplate;
			}
			Configuration.Save();
			LastTellTarget = state.LastTellTarget ?? LastTellTarget;
			SaveEntries();
			Log.Information($"Restored session snapshot: entries={list.Count}, startMil={(Configuration.ActiveSessionStartingPotMillions.HasValue ? Configuration.ActiveSessionStartingPotMillions.Value.ToString() : "(none)")}, bonusLeft={BonusTicketsRemaining}, vipTracked={num}.", Array.Empty<object>());
			ChatGui.Print("↩ Restored previous raffle session after unexpected shutdown.", (string)null, (ushort?)null);
			return result;
		}
		catch (Exception ex)
		{
			Log.Error("Failed to restore session: " + ex.Message, Array.Empty<object>());
			return false;
		}
	}

	public string GenerateCsvFromTickets()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("TicketNumber,PlayerName");
		int num = 1;
		foreach (TicketEntry entry in Entries)
		{
			for (int i = 0; i < entry.TotalTickets; i++)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(1, 2, stringBuilder2);
				handler.AppendFormatted(num++);
				handler.AppendLiteral(",");
				handler.AppendFormatted(entry.PlayerName);
				stringBuilder2.AppendLine(ref handler);
			}
		}
		return stringBuilder.ToString();
	}
}
