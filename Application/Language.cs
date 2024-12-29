namespace Boggle;

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
    private static string languageCode;

    /// <summary>
    /// Sorted list of letters, their associated points, and their probabilities.
    /// </summary>
    private static SortedList<char, int> pointsPerLetter;

    /// <summary>
    /// Sorted list of letters and their maximum occurrences.
    /// </summary>
    private static SortedList<char, int> maxOccurrencesPerLetter;

    /// <summary>
    /// Array of letters, each appearing as many times as their probability.
    /// </summary>
    private static char[] letterProbabilities = new char[100];

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Initializes the game language.
    /// </summary>
    /// <param name="language">Game language</param>
    public static void Initialize(string language)
    {
        FindLanguageCode(language);

        InitializeDataStructures();

        ReadFile();
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the game language code.
    /// </summary>
    public static string LanguageCode
    {
        get { return Language.languageCode; }
    }

    /// <summary>
    /// Gets or sets the list of maximum occurrences per letter.
    /// </summary>
    public static SortedList<char, int> MaxOccurrences
    {
        get { return Language.maxOccurrencesPerLetter; }
        set { Language.maxOccurrencesPerLetter = value; }
    }

    /// <summary>
    /// Gets the list of points per letter.
    /// </summary>
    public static SortedList<char, int> PointsPerLetter
    {
        get { return Language.pointsPerLetter; }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Returns a specific letter based on a randomly generated input.
    /// </summary>
    /// <param name="probability">Number between 0 and 100 exclusive.</param>
    /// <returns>Randomly generated letter considering the probabilities of each.</returns>
    public static char GetLetterByProbability(int probability)
    {
        return letterProbabilities[probability];
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
        foreach (var element in LANGUAGES)
        {
            if (element.Contains(language))
            {
                Language.languageCode = element[0];
                break;
            }
        }
        if (Language.languageCode == null)
        {
            throw new ArgumentException("Language " + language + " not recognized. (fr, en)", language);
        }
    }

    /// <summary>
    /// Initializes several data structures that will receive various information from the letter file.
    /// </summary>
    private static void InitializeDataStructures()
    {
        Language.pointsPerLetter = new();
        Language.maxOccurrencesPerLetter = new();
        CustomDictionary.Initialize();
    }

    /// <summary>
    /// Reads the file.
    /// </summary>
    private static void ReadFile()
    {
        try
        {
            string[] file = File.ReadAllLines("Files/Letters_" + languageCode + ".txt");

            int index = 0;

            foreach (string letter in file)
            {
                string[] data = letter.Split(';', 3);
                char key = Convert.ToChar(data[0]);
                int points = Convert.ToInt32(data[1]);
                int occurrences = Convert.ToInt32(data[2]);

                pointsPerLetter.Add(key, points);
                maxOccurrencesPerLetter.Add(key, occurrences);

                for (int i = 0; i < occurrences; i++)
                {
                    letterProbabilities[index] = key;
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

    #endregion Methods
}