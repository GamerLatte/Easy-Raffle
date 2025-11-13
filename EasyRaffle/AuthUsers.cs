using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasyRaffle;

public class AuthUsers
{
	public class AllowedUserList
	{
		public List<string> Allowed { get; set; } = new List<string>();
	}

	private static readonly string AllowedUsersUrl = "https://raw.githubusercontent.com/nilah-xiv/RafflerUsers/main/allowed_users.json";

	public List<string> AllowedUsers { get; private set; } = new List<string>();

	public async Task LoadAllowedUsersAsync()
	{
		try
		{
			using HttpClient client = new HttpClient();
			AllowedUsers = JsonSerializer.Deserialize<AllowedUserList>(await client.GetStringAsync(AllowedUsersUrl))?.Allowed ?? new List<string>();
			Plugin.Log.Info($"Loaded {AllowedUsers.Count} allowed users.", Array.Empty<object>());
		}
		catch (Exception ex)
		{
			Plugin.Log.Error("Failed to load allowed users: " + ex.Message, Array.Empty<object>());
		}
	}
}
