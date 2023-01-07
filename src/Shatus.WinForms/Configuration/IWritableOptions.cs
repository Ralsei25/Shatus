using Microsoft.Extensions.Options;

namespace Shatus.WinForms.Configs;

public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
{
    Task UpdateAsync(Action<T> applyChanges);
}
