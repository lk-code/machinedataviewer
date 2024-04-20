using System.Text.Json;
using DataViewer.Core.Contracts;

namespace DataViewer.Core;

public class SettingsStorage(IIOProvider iOProvider) : ISettingsStorage
{
    private readonly string SETTINGS_FILE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DataViewer", "settings.json");

    /// <inheritdoc />
    public async Task Write(string key, dynamic value, CancellationToken cancellationToken)
    {
        Dictionary<string, dynamic> settings = new();

        if (!File.Exists(SETTINGS_FILE_PATH))
        {
            await iOProvider.CreateDirectoryAsync(Path.GetDirectoryName(SETTINGS_FILE_PATH)!, cancellationToken);

            await Task.Delay(250, cancellationToken);

            var settingsJson = JsonSerializer.Serialize(settings);
            await iOProvider.WriteFileContentAsync(SETTINGS_FILE_PATH, settingsJson, cancellationToken);
        }
        else
        {
            settings = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(await iOProvider.GetFileContentAsync(SETTINGS_FILE_PATH, cancellationToken))!;
        }

        if (settings.ContainsKey(key))
        {
            settings[key] = value;
        }
        else
        {
            settings.Add(key, value);
        }

        string jsonContent = JsonSerializer.Serialize(settings);

        await iOProvider.WriteFileContentAsync(SETTINGS_FILE_PATH, jsonContent, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> Read<T>(string key, CancellationToken cancellationToken)
    {
        if (!File.Exists(SETTINGS_FILE_PATH))
        {
            return default;
        }

        string jsonContent = await iOProvider.GetFileContentAsync(SETTINGS_FILE_PATH, cancellationToken);
        Dictionary<string, dynamic>? settings = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(jsonContent);

        if (settings is null)
        {
            return default;
        }

        JsonElement? value = settings[key];

        if (value is null
            || value.Value.ValueKind == JsonValueKind.Null)
        {
            return default;
        }

        T? settingsValue = JsonSerializer.Deserialize<T>(value.Value.GetRawText());
        return settingsValue;
    }
}
