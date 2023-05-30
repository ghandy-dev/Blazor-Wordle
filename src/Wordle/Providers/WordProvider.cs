namespace Wordle.Providers;

public class WordProvider : IWordProvider
{
    private readonly HashSet<string> _wordsHashSet;
    private readonly string[] _words;

    /// <summary>
    /// Initialises a new instance of the WordProvider class.
    /// </summary>
    /// <param name="words">Words to use as both a dictionary and as a word list to choose a word from.</param>
    public WordProvider(string[] words)
        : this(words, words)
    { }

    //// <summary>
    /// Initialises a new instance of the WordProvider class.
    /// </summary>
    /// <param name="dictionary">Word dictionary validate guesses against.</param>
    /// <param name="words">Subset of words from the dictionary.</param>
    public WordProvider(string[] dictionary, string[] words)
    {
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
        if (words == null) throw new ArgumentNullException(nameof(words));

        if (dictionary.Length == 0) throw new ArgumentException("empty array of words", nameof(dictionary));
        if (words.Length == 0) throw new ArgumentException("empty array of words", nameof(words));

        _words = words.Select(x => x.ToUpper()).ToArray();
        _wordsHashSet = dictionary.Select(x => x.ToUpper()).ToHashSet();
    }

    public bool CheckWord(string word)
    {
        return _wordsHashSet.Contains(word.ToUpper());
    }

    public string GetTodaysWordle()
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var hash = date.GetHashCode();
        return _words[new Random(hash).Next(0, _words.Length)];
    }

    public string GetPreviousWordle(DateOnly date)
    {
        var hash = date.GetHashCode();
        return _words[new Random(hash).Next(0, _words.Length)];
    }
}