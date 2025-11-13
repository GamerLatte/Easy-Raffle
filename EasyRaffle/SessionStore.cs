using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace EasyRaffle;

public sealed class SessionStore : IDisposable
{
	private readonly string sessionPath;

	private readonly string backupDir;

	private readonly Timer timer;

	private readonly JsonSerializerOptions jsonOptions;

	private readonly object gate = new object();

	private bool pendingSave;

	private readonly bool hadCrashLastRun;

	public SessionState State { get; private set; } = new SessionState();

	public SessionStore(string baseDir)
	{
		Directory.CreateDirectory(baseDir);
		sessionPath = Path.Combine(baseDir, "EasyRaffleSession.json");
		backupDir = Path.Combine(baseDir, "EasyRaffleSession.backups");
		Directory.CreateDirectory(backupDir);
		jsonOptions = new JsonSerializerOptions
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};
		LoadOrCreate();
		hadCrashLastRun = !State.ClosedCleanly;
		State.ClosedCleanly = false;
		SaveAtomic();
		timer = new Timer(delegate
		{
			if (pendingSave)
			{
				SaveAtomic();
			}
		}, null, 3000, 3000);
	}

	public void Update(Action<SessionState> mutate)
	{
		lock (gate)
		{
			mutate(State);
			pendingSave = true;
		}
	}

	public void MarkCleanAndSave()
	{
		lock (gate)
		{
			State.ClosedCleanly = true;
			SaveAtomic();
		}
	}

	public bool HadCrashLastRun()
	{
		return hadCrashLastRun;
	}

	private void LoadOrCreate()
	{
		try
		{
			if (File.Exists(sessionPath))
			{
				string json = File.ReadAllText(sessionPath, Encoding.UTF8);
				State = JsonSerializer.Deserialize<SessionState>(json, jsonOptions) ?? new SessionState();
			}
			else
			{
				State = new SessionState();
			}
		}
		catch
		{
			try
			{
				string text = DateTimeOffset.UtcNow.ToString("yyyyMMdd_HHmmss");
				File.Copy(sessionPath, Path.Combine(backupDir, "EasyRaffleSession.corrupt_" + text + ".json"), overwrite: true);
			}
			catch
			{
			}
			State = new SessionState();
		}
	}

	private void SaveAtomic()
	{
		lock (gate)
		{
			State.SavedAt = DateTimeOffset.UtcNow;
			string text = sessionPath + ".tmp";
			string contents = JsonSerializer.Serialize(State, jsonOptions);
			File.WriteAllText(text, contents, Encoding.UTF8);
			File.Copy(text, sessionPath, overwrite: true);
			File.Delete(text);
			pendingSave = false;
			try
			{
				string text2 = DateTimeOffset.UtcNow.ToString("yyyyMMdd_HHmmss");
				File.WriteAllText(Path.Combine(backupDir, "EasyRaffleSession_" + text2 + ".json"), contents, Encoding.UTF8);
				FileInfo[] files = new DirectoryInfo(backupDir).GetFiles("EasyRaffleSession_*.json");
				Array.Sort(files, (FileInfo a, FileInfo b) => b.CreationTimeUtc.CompareTo(a.CreationTimeUtc));
				for (int num = 3; num < files.Length; num++)
				{
					files[num].Delete();
				}
			}
			catch
			{
			}
		}
	}

	public void Dispose()
	{
		try
		{
			MarkCleanAndSave();
		}
		catch
		{
		}
		timer?.Dispose();
	}
}
