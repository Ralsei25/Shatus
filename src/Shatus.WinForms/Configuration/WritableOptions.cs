using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Shatus.WinForms.Configs;

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    private readonly string _optionsDirectoryPath;
    private readonly IOptionsMonitor<T> _options;
    private readonly string _section;
    private readonly string _optionsFileName;

    public WritableOptions(string configsDirectoryPath, IOptionsMonitor<T> options, string section, string file)
    {
        _optionsDirectoryPath = configsDirectoryPath;
        _options = options;
        _section = section;
        _optionsFileName = file;
    }

    public T Value => _options.CurrentValue;
    public T Get(string? name) => _options.Get(name);

    public async Task UpdateAsync(Action<T> applyChanges)
    {
        var filePath = Path.Combine(_optionsDirectoryPath, _optionsFileName);
        if (!File.Exists(filePath))
            await File.Create(filePath).DisposeAsync();

        var jsonObject = JsonSerializer.Deserialize<JsonObject>(await File.ReadAllTextAsync(filePath)) ?? new JsonObject();

        var sectionObject = jsonObject.TryGetPropertyValue(_section, out var section) 
                ? JsonSerializer.Deserialize<T>(section!.ToString()) ?? new T()
                : Value ?? new T();

        applyChanges(sectionObject);

        jsonObject[_section] = JsonNode.Parse(JsonSerializer.Serialize(sectionObject));
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(jsonObject, new JsonSerializerOptions { WriteIndented = true }));
    }
}
