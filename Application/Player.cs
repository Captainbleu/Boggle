namespace Boggle;

/// <summary>
/// Represents a player in the game.
/// </summary>
public class Player
{
    #region Fields

    /// <summary>
    /// Name of the player.
    /// </summary>
    private string name;

    /// <summary>
    /// Score of the player.
    /// </summary>
    private int score;

    /// <summary>
    /// Dictionary of words found by the player.
    /// <list type="table">
    /// <item>Key: the word found.</item>
    /// <item>Value: the number of occurrences of the word.</item>
    /// </list>
    /// </summary>
    private readonly Dictionary<string, int> foundWords;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// <para>A player can only be created if they have a defined and unique name.</para>
    /// </summary>
    /// <remarks>
    /// Initialized at the beginning of the game, their <c>score</c> is initially zero and their list of <c>words</c> found is empty.
    /// </remarks>
    /// <param name="name">Name of the player.</param>
    public Player(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("The player's name cannot be empty or null. (" + name + ")");
        }

        string cleanedName = name.Trim(" ;§%*µ$£^¨`@=+".ToCharArray()).ToUpper();
        if (string.IsNullOrWhiteSpace(cleanedName))
        {
            throw new ArgumentException("The player's name cannot be empty or null. (" + name + ")");
        }

        if (DoesPlayerNameExist(cleanedName))
        {
            throw new ArgumentException("The player's name must be unique. (" + name + ")");
        }

        this.name = cleanedName;
        this.score = 0;
        this.foundWords = new Dictionary<string, int>();
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public string Name
    {
        get { return this.name; }
    }

    /// <summary>
    /// Gets the score of the player.
    /// </summary>
    public int Score
    {
        get { return this.score; }
    }

    /// <summary>
    /// Gets the list of words found by the player along with their occurrences.
    /// </summary>
    public Dictionary<string, int> FoundWords
    {
        get { return this.foundWords; }
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
        return this.foundWords.ContainsKey(word);
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
        if (!this.foundWords.TryAdd(word, 1))
        {
            foundWords[word]++;
        }

        this.score += Language.CalculateScore(word);
    }

    /// <summary>
    /// Returns a string describing the player.
    /// </summary>
    /// <returns>Description of the player by their name, score, and number of words found.</returns>
    public override string ToString()
    {
        return "Player: " + this.name + ", Score: " + this.score + ", Number of words found: " + this.foundWords.Count + ".";
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