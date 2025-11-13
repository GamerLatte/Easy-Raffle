using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;

namespace Raffler;

public class AuthorizationManager : IDisposable
{
	public class AllowedUserList
	{
		[JsonPropertyName("allowed")]
		public List<string> Allowed { get; set; } = new List<string>();
	}

	private readonly IFramework framework;

	private readonly IClientState clientState;

	private readonly IPluginLog log;

	private readonly IChatGui chatGui;

	private bool initialized;

	private List<string> allowedUsers = new List<string>();

	private const string WhitelistUrl = "https://raw.githubusercontent.com/nilah-xiv/RafflerUsers/main/allowed_users.json";

	public bool Initialized => initialized;

	public bool IsAuthorizedUser { get; private set; }

	public AuthorizationManager(IFramework framework, IClientState clientState, IPluginLog log, IChatGui chatGui)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		this.framework = framework;
		this.clientState = clientState;
		this.log = log;
		this.chatGui = chatGui;
		clientState.Login += OnLogin;
		framework.Update += new OnUpdateDelegate(OnFrameworkUpdate);
		LoadWhitelistAsync();
	}

	private void OnLogin()
	{
		if (clientState.LocalPlayer != null && allowedUsers.Count != 0)
		{
			string playerName = ((IGameObject)clientState.LocalPlayer).Name.TextValue;
			log.Info("\ud83d\udd04 Rechecking auth on login: " + playerName, Array.Empty<object>());
			IsAuthorizedUser = allowedUsers.Any((string name) => string.Equals(name.Trim(), playerName.Trim(), StringComparison.OrdinalIgnoreCase));
			log.Info(IsAuthorizedUser ? " Player is authorized after login." : " Player is NOT authorized after login.", Array.Empty<object>());
			initialized = true;
		}
	}

	private async Task LoadWhitelistAsync()
	{
		try
		{
			using HttpClient client = new HttpClient();
			allowedUsers = JsonSerializer.Deserialize<AllowedUserList>(await client.GetStringAsync("https://raw.githubusercontent.com/nilah-xiv/RafflerUsers/main/allowed_users.json"))?.Allowed ?? new List<string>();
			Plugin.Log.Info($"Loaded {allowedUsers.Count} authorized users.", Array.Empty<object>());
		}
		catch (Exception)
		{
		}
		Plugin.Log.Error("Failed to load whitelist:", Array.Empty<object>());
		chatGui.PrintError("Failed to load authorized users list. Please try again later.", (string)null, (ushort?)null);
	}

	private void OnFrameworkUpdate(IFramework _)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		if (!initialized && clientState.LocalPlayer != null && allowedUsers.Count != 0)
		{
			string playerName = ((IGameObject)clientState.LocalPlayer).Name.TextValue;
			log.Info("Detected player: '" + playerName + "'", Array.Empty<object>());
			log.Info("Comparing to allowed list", Array.Empty<object>());
			if (allowedUsers.Any((string name) => string.Equals(name.Trim(), playerName.Trim(), StringComparison.OrdinalIgnoreCase)))
			{
				IsAuthorizedUser = true;
				log.Info("Player is authorized.", Array.Empty<object>());
			}
			else
			{
				chatGui.PrintError("You are not authorized to use Raffler.", (string)null, (ushort?)null);
				log.Warning("Player not found in allowed list.", Array.Empty<object>());
			}
			initialized = true;
			framework.Update -= new OnUpdateDelegate(OnFrameworkUpdate);
		}
	}

	public void Dispose()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		framework.Update -= new OnUpdateDelegate(OnFrameworkUpdate);
	}
}
