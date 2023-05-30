namespace Wordle.Blazor.Utilities;

public interface IKeyGenerator
{
    string Generate();

    IEnumerable<string> GenerateMultipleKeys(int count);
}
