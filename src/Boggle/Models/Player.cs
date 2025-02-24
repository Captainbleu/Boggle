namespace Boggle.Models;

/// <summary>
/// Represents a player in the game.
/// </summary>
public class Player
{
    #region Fields

    /// <summary>
    /// Name of the player.
    /// </summary>
    private readonly string _name;

    /// <summary>
    /// Score of the player.
    /// </summary>
    private int _score;

    /// <summary>
    /// Dictionary of words found by the player.
    /// <list type="table">
    /// <item>Key: the word found.</item>
    /// <item>Value: the number of occurrences of the word.</item>
    /// </list>
    /// </summary>
    private Dictionary<string, int> _foundWords;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    /// <remarks>
    /// Initialized at the beginning of the game, their <c>score</c> is initially zero and their list of <c>words</c> found is empty.
    /// </remarks>
    /// <param name="name">Name of the player.</param>
    public Player(string name)
    {
        _name = name;
        _score = 0;
        _foundWords = new Dictionary<string, int>();
    }

    /// <summary>
    /// Attempts to add a player with the specified name.
    /// </summary>
    /// <param name="name">The name of the player to add.</param>
    /// <param name="errorMessage">An error message if the player could not be added.</param>
    /// <returns><c>true</c> if the player was added successfully; otherwise, <c>false</c>.</returns>
    public static bool TryAddPlayer(string name, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            errorMessage = "The player's name cannot be empty or null. (" + name + ")";
            return false;
        }

        string cleanedName = name.Trim(" ;§%*µ$£^¨`@=+".ToCharArray()).ToUpper();
        if (string.IsNullOrWhiteSpace(cleanedName))
        {
            errorMessage = "The player's name cannot be empty or null. (" + name + ")";
            return false;
        }

        if (DoesPlayerNameExist(cleanedName))
        {
            errorMessage = "The player's name must be unique. (" + name + ")";
            return false;
        }

        Game.PlayerList.Add(new Player(cleanedName));

        return true;
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public string Name
    {
        get { return _name; }
    }

    /// <summary>
    /// Gets the score of the player.
    /// </summary>
    public int Score
    {
        get { return _score; }
    }

    /// <summary>
    /// Gets the list of words found by the player along with their occurrences.
    /// </summary>
    public Dictionary<string, int> FoundWords
    {
        get { return _foundWords; }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Checks if the specified word has already been found by the player.
    /// </summary>
    /// <param name="word">Word to check.</param>
    /// <returns><c>true</c> if the word has already been found, otherwise <c>false</c>.</returns>
    public bool Contains(string word)
    {
        return _foundWords.ContainsKey(word);
    }

    /// <summary>
    /// Updates the player's score and the list of words found by the player.
    /// </summary>
    /// <remarks>
    /// Attempts to add the word with a value of 1 using the <see cref="Dictionary{TKey,TValue}.TryAdd">TryAdd</see> method.
    /// <list type="bullet">
    /// <item>If <c>true</c>, the word was added with a value of 1.</item>
    /// <item>If <c>false</c>, increments the word's value by 1.</item>
    /// </list>
    /// Then increments the player's score based on the length of the word and the value of each letter.
    /// </remarks>
    /// <param name="word">Word to add.</param>
    /// <seealso cref="foundWords"/>
    public void AddWord(string word)
    {
        word = word.ToUpper();
        if (!_foundWords.TryAdd(word, 1))
        {
            _foundWords[word]++;
        }

        _score += Language.CalculateScore(word);
    }

    /// <summary>
    /// Returns a string describing the player.
    /// </summary>
    /// <returns>Description of the player by their name, score, and number of words found.</returns>
    public override string ToString()
    {
        return "Player: " + _name + ", Score: " + _score + ", Number of words found: " + _foundWords.Count + ".";
    }

    /// <summary>
    /// Checks if a player with the specified name already exists.
    /// </summary>
    /// <param name="name">Name of the player.</param>
    /// <returns><c>true</c> if the player is already in the list of players, otherwise <c>false</c>.</returns>
    private static bool DoesPlayerNameExist(string name)
    {
        foreach (Player player in Game.PlayerList)
        {
            if (player.Name == name)
            {
                return true;
            }
        }

        return false;
    }

    #endregion Methods
}