using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Shatus.WinForms.Configs;

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    private readonly string _configsDirectoryPath;
    private readonly IOptionsMonitor<T> _options;
    private readonly string _section;
    private readonly string _file;

    public WritableOptions(string configsDirectoryPath,
        IOptionsMonitor<T> options,
        string section,
        string file)
    {
        _configsDirectoryPath = configsDirectoryPath;
        _options = options;
        _section = section;
        _file = file;
    }

    public T Value => _options.CurrentValue;
    public T Get(string name) => _options.Get(name);

    public async Task UpdateAsync(Action<T> applyChanges)
    {
        var filePath = Path.Combine(_configsDirectoryPath, _file);

        if (!File.Exists(filePath))
            await File.Create(filePath).DisposeAsync();

        JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(await File.ReadAllTextAsync(filePath)) ?? new JsonObject();
        var sectionObject = jObject.TryGetPropertyValue(_section, out var section) ?
            JsonSerializer.Deserialize<T>(section!.ToString()) : (Value ?? new T());

        applyChanges(sectionObject);

        jObject[_section] = JsonNode.Parse(JsonSerializer.Serialize(sectionObject));
        await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(jObject, new JsonSerializerOptions { WriteIndented = true }));
    }
}
