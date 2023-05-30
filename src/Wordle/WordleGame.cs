using System.Reactive;
using System.Reactive.Subjects;

using Wordle.Providers;

using static Wordle.Extensions.IntExtensions;

namespace Wordle;

public class WordleGame
{
    public List<GuessModel> Guesses { get; private set; } = new();

    public GameStatus Status { get; private set; }

    public string Word { get; }

    public int MaxGuesses { get; }

    public bool HardMode { get; set; }

    public Keyboard Keyboard { get; }

    public int GuessCount => Guesses.Count;

    public GuessModel? LatestGuess => Guesses.LastOrDefault();

    public IObservable<GameEvent> GameEventStream => _gameEventSubject;

    private readonly Subject<GameEvent> _gameEventSubject = new();
    private readonly IWordProvider _wordProvider;

    public WordleGame(int maxGuesses, IWordProvider wordProvider) : this(maxGuesses, wordProvider, false)
    { }

    public WordleGame(int maxGuesses, IWordProvider wordProvider, bool hardMode)
    {
        Keyboard = new Keyboard();
        MaxGuesses = maxGuesses;
        HardMode = hardMode;
        _wordProvider = wordProvider;
        Word = _wordProvider.GetTodaysWordle();
    }

    public void LoadState(List<GuessModel> guesses)
    {
        Guesses = guesses;

        if (Guesses.Count > 0)
        {
            if (guesses[^1].LetterStates.All(state => state == LetterState.Correct))
            {
                Status = GameStatus.Win;
            }
            else if (Guesses.Count == MaxGuesses)
            {
                Status = GameStatus.Loss;
            }
        }

        foreach (var guess in guesses)
        {
            UpdateLetterStatesFromGuessModel(guess);
        }
    }

    public void MakeGuess(string guess)
    {
        guess = guess.ToUpper();
        bool valid = true;
        string? error = null;

        if (Status == GameStatus.Loss || Status == GameStatus.Win)
        {
            error = "Game is over";
            valid = false;
        }
        else if (Status == GameStatus.InProgress)
        {
            if (Guesses.Count > MaxGuesses)
            {
                error = "Max number of guesses already made";
                valid = false;
            }
            else if (guess.Count(x => !char.IsWhiteSpace(x)) < Word.Length)
            {
                error = "Not enough letters";
                valid = false;
            }
            else if (guess.Count(x => !char.IsWhiteSpace(x)) > Word.Length)
            {
                error = "Too many letters";
                valid = false;
            }
            else if (!_wordProvider.CheckWord(guess))
            {
                error = "Not in word list";
                valid = false;
            }
            else if (HardMode)
            {
                if (!ValidateLetterPositions(guess, out var hardModeError))
                {
                    error = hardModeError!;
                    valid = false;
                }
                else if (!ValidateAllLettersUsed(guess, out hardModeError))
                {
                    error = hardModeError!;
                    valid = false;
                }
            }
        }

        if (!valid)
        {
            _gameEventSubject.OnNext(new GameEvent
            {
                GameEventType = GameEventType.InvalidGuess,
                Data = error,
            });
        }
        else
        {
            var guessModel = CreateGuessModel(guess);
            var correct = guessModel.LetterStates.All(state => state == LetterState.Correct);

            Guesses.Add(guessModel);
            UpdateLetterStatesFromGuessModel(guessModel);

            if (correct)
            {
                SetStatusCompleted(GameStatus.Win);
            }
            else if (Guesses.Count == MaxGuesses)
            {
                SetStatusCompleted(GameStatus.Loss);
            }

            _gameEventSubject.OnNext(new GameEvent
            {
                GameEventType = GameEventType.GuessMade,
                Data = guessModel,
            });
        }
    }

    private void UpdateLetterStatesFromGuessModel(GuessModel guess)
    {
        foreach (var (letter, state) in guess.GuessedWord.Zip(guess.LetterStates))
        {
            var key = Keyboard[letter];
            if (state > key.State)
            {
                key.State = state;
            }
        }
    }

    private void SetStatusCompleted(GameStatus newStatus)
    {
        Status = newStatus;
        _gameEventSubject.OnNext(new GameEvent
        {
            GameEventType = GameEventType.GameEnded,
            Data = null,
        });
    }

    private bool ValidateLetterPositions(string guess, out string? error)
    {
        error = null;

        for (int i = 0; i < LatestGuess?.GuessedWord.Length; i++)
        {
            if (LatestGuess.LetterStates[i] == LetterState.Correct &&
                LatestGuess.GuessedWord[i] != guess[i])
            {
                error = $"{(i + 1).ToOrdinalString()} letter must be {LatestGuess?.GuessedWord[i]}";
                return false;
            }
        }

        return true;
    }

    private bool ValidateAllLettersUsed(string guess, out string? error)
    {
        error = null;

        if (LatestGuess is not null)
        {
            foreach (var (letter, state) in LatestGuess.GuessedWord.Zip(LatestGuess.LetterStates))
            {
                if (state == LetterState.Present && !guess.Contains(letter))
                {
                    error = $"Guess must contain {letter}";
                    return false;
                }
            }
        }

        return true;
    }

    private GuessModel CreateGuessModel(string guess)
    {
        var letterCounts =
            (from word in Word
             group word by word into grp
             select new { grp.Key, Count = grp.Count() })
            .ToDictionary(x => x.Key, x => x.Count);

        var states = guess.Select((letter, index) =>
        {
            var state = (Word.Contains(letter) && letterCounts[letter] > 0) switch
            {
                true => (Word[index] == letter) switch
                {
                    true => LetterState.Correct,
                    false => LetterState.Present,
                },
                false => LetterState.Absent,
            };

            if (state == LetterState.Correct || state == LetterState.Present)
            {
                letterCounts[letter]--;
            }

            return state;
        }).ToList();

        return new GuessModel
        {
            GuessedWord = guess,
            LetterStates = states,
        };
    }
}