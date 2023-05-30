namespace Wordle.Blazor.Utilities;

public class KeyGenerator : IKeyGenerator
{
    public string Generate() => Guid.NewGuid().ToString();

    public IEnumerable<string> GenerateMultipleKeys(int count) =>
         Enumerable.Repeat(() => Generate(), count).Select(k => k());
}