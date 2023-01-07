using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shatus.WinForms.Configs;

public class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    private readonly IOptionsMonitor<T> _options;
    private readonly string _section;
    private readonly string _file;

    public WritableOptions(
        IOptionsMonitor<T> options,
        string section,
        string file)
    {
        _options = options;
        _section = section;
        _file = file;
    }

    public T Value => _options.CurrentValue;
    public T Get(string name) => _options.Get(name);

    public async Task UpdateAsync(Action<T> applyChanges)
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, _file);

        if (!File.Exists(filePath))
            await File.Create(filePath).DisposeAsync();

        var jObject = JsonConvert.DeserializeObject<JObject>(await File.ReadAllTextAsync(filePath));
        var sectionObject = jObject?.TryGetValue(_section, out JToken? section) == true ?
            JsonConvert.DeserializeObject<T>(section.ToString()) : (Value ?? new T());

        applyChanges(sectionObject);

        jObject[_section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
        await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
    }
}
