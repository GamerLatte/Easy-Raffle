using System;
using Dalamud.Plugin.Services;

namespace EasyRaffle;

public class AuthorizationManager : IDisposable
{
	private readonly IFramework framework;

	private readonly IClientState clientState;

	private readonly IPluginLog log;

	private readonly IChatGui chatGui;

	private bool initialized;

	public bool Initialized => initialized;

	public bool IsAuthorizedUser { get; private set; }

	public AuthorizationManager(IFramework framework, IClientState clientState, IPluginLog log, IChatGui chatGui)
	{
		this.framework = framework;
		this.clientState = clientState;
		this.log = log;
		this.chatGui = chatGui;
		clientState.Login += OnLogin;
		framework.Update += new OnUpdateDelegate(OnFrameworkUpdate);
		// Authorization removed - all users are authorized
		IsAuthorizedUser = true;
		initialized = true;
		log.Info("Authorization bypassed - all users authorized.", Array.Empty<object>());
	}

	private void OnLogin()
	{
		// Authorization removed - always authorized
		IsAuthorizedUser = true;
		initialized = true;
	}

	private void OnFrameworkUpdate(IFramework _)
	{
		// Authorization removed - always authorized
		if (!initialized && clientState.LocalPlayer != null)
		{
			IsAuthorizedUser = true;
			initialized = true;
			framework.Update -= new OnUpdateDelegate(OnFrameworkUpdate);
		}
	}

	public void Dispose()
	{
		framework.Update -= new OnUpdateDelegate(OnFrameworkUpdate);
	}
}
