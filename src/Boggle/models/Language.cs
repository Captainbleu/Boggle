namespace Boggle.models;

/// <summary>
/// Stores information related to the language.
/// </summary>
public static class Language
{
    private static List<string[]> LANGUAGES = new()
    {
        { ["french", "français", "francais", "fr"] },
        { ["english", "anglais", "en", "an"] }
    };

    #region Fields

    /// <summary>
    /// Code of the chosen language.
    /// </summary>
    private static string _languageCode = string.Empty;

    /// <summary>
    /// Sorted list of letters, their associated points, and their probabilities.
    /// </summary>
    private static SortedList<char, int> _pointsPerLetter = new();

    /// <summary>
    /// Sorted list of letters and their maximum occurrences.
    /// </summary>
    private static SortedList<char, int> _maxOccurrencesPerLetter = new();

    /// <summary>
    /// Array of letters, each appearing as many times as their probability.
    /// </summary>
    private static char[] _letterProbabilities = new char[100];

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the game language code.
    /// </summary>
    public static string LanguageCode
    {
        get { return _languageCode; }
    }

    /// <summary>
    /// Gets or sets the list of maximum occurrences per letter.
    /// </summary>
    public static SortedList<char, int> MaxOccurrences
    {
        get { return _maxOccurrencesPerLetter; }
        set { _maxOccurrencesPerLetter = value; }
    }

    /// <summary>
    /// Gets the list of points per letter.
    /// </summary>
    public static SortedList<char, int> PointsPerLetter
    {
        get { return _pointsPerLetter; }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Initializes the game language.
    /// </summary>
    /// <param name="language">Game language</param>
    public static void Initialize(string language)
    {
        FindLanguageCode(language);

        CustomDictionary.Initialize();

        InitializeDataStructures();
    }

    /// <summary>
    /// Returns a specific letter based on a randomly generated input.
    /// </summary>
    /// <param name="probability">Number between 0 and 100 exclusive.</param>
    /// <returns>Randomly generated letter considering the probabilities of each.</returns>
    public static char GetLetterByProbability(int probability)
    {
        return _letterProbabilities[probability];
    }

    /// <summary>
    /// Cleans the input string and attempts to retrieve the game language.
    /// </summary>
    /// <param name="language">Language chosen by the player.</param>
    /// <exception cref="ArgumentException">
    /// Invalid language. Supported languages:
    /// <list type="bullet">
    /// <item>English (en)</item>
    /// <item>French (fr)</item>
    /// </list>
    /// </exception>
    private static void FindLanguageCode(string language)
    {
        language = language.Trim(" ,?;.:/!§%*µ$£^¨<>&~#{([-|`_@)]=}+°".ToCharArray()).ToLower();

        var match = LANGUAGES.FirstOrDefault(x => x.Contains(language));
        if (match != null)
        {
            _languageCode = match[0];
        }
        if (_languageCode == null)
        {
            throw new ArgumentException("Language " + language + " not recognized. (fr, en)", language);
        }
    }

    /// <summary>
    /// Initializes several data structures with information from the letter file.
    /// </summary>
    private static void InitializeDataStructures()
    {
        try
        {
            string[] file = File.ReadAllLines("data/Letters_" + _languageCode + ".txt");

            int index = 0;

            foreach (string letter in file)
            {
                string[] data = letter.Split(';', 3);
                char key = Convert.ToChar(data[0]);
                int points = Convert.ToInt32(data[1]);
                int occurrences = Convert.ToInt32(data[2]);

                _pointsPerLetter.Add(key, points);
                _maxOccurrencesPerLetter.Add(key, occurrences);

                for (int i = 0; i < occurrences; i++)
                {
                    _letterProbabilities[index] = key;
                    index++;
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("The file was not found.");
            Console.WriteLine(e.Message);
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while reading the file.");
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Calculates the score for a given word.
    /// </summary>
    /// <param name="word">Word to calculate the score for.</param>
    /// <returns>Score of the word.</returns>
    public static int CalculateScore(string word)
    {
        int wordScore = 0;
        foreach (char letter in word)
        {
            wordScore += PointsPerLetter[letter];
        }

        wordScore += word.Length * (int)Math.Ceiling(Math.Log2(word.Length));

        return wordScore;
    }

    #endregion Methods
}