using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public static class DiscordWebhookHelper
{
	public class WebhookMessage
	{
		public string Id { get; set; } = "";
	}

	public static async Task<string?> SendCsvToDiscord(string webhookUrl, string fileName, string csvContent)
	{
		using HttpClient client = new HttpClient();
		using MultipartFormDataContent content = new MultipartFormDataContent();
		ByteArrayContent byteArrayContent = new ByteArrayContent(Encoding.UTF8.GetBytes(csvContent));
		byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/csv");
		content.Add(byteArrayContent, "file", fileName);
		HttpResponseMessage obj = await client.PostAsync(webhookUrl + "?wait=true", content);
		obj.EnsureSuccessStatusCode();
		return JsonSerializer.Deserialize<WebhookMessage>(await obj.Content.ReadAsStringAsync())?.Id;
	}

	public static async Task<string?> SendTextToDiscord(string webhookUrl, string content)
	{
		using HttpClient client = new HttpClient();
		string content2 = JsonSerializer.Serialize(new { content });
		HttpResponseMessage obj = await client.PostAsync(webhookUrl + "?wait=true", new StringContent(content2, Encoding.UTF8, "application/json"));
		obj.EnsureSuccessStatusCode();
		return JsonSerializer.Deserialize<WebhookMessage>(await obj.Content.ReadAsStringAsync())?.Id;
	}

	public static async Task EditWebhookMessage(string webhookUrl, string messageId, string newContent)
	{
		using HttpClient client = new HttpClient();
		string content = JsonSerializer.Serialize(new
		{
			content = newContent
		});
		HttpRequestMessage request = new HttpRequestMessage
		{
			Method = HttpMethod.Patch,
			RequestUri = new Uri(webhookUrl + "/messages/" + messageId),
			Content = new StringContent(content, Encoding.UTF8, "application/json")
		};
		(await client.SendAsync(request)).EnsureSuccessStatusCode();
	}
}
