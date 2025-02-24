namespace Boggle.Models;

/// <summary>
/// Represents a dictionary of words for a given language.
/// <para>Provides methods for more efficient word search.</para>
/// </summary>
public static class CustomDictionary
{
    #region Fields

    /// <summary>
    /// List of words sorted by length.
    /// <list type="table">
    /// <item>Key: length.</item>
    /// <item>Value: words of that length.</item>
    /// </list>
    /// </summary>
    private static readonly SortedDictionary<int, string[]> _wordsByLength = new();

    /// <summary>
    /// List of words sorted by first letter.
    /// <list type="table">
    /// <item>Key: first letter.</item>
    /// <item>Value: words starting with that letter.</item>
    /// </list>
    /// </summary>
    private static readonly SortedDictionary<char, string[]> _wordsByLetter = new();

    /// <summary>
    /// Distribution of the number of words by length.
    /// <list type="table">
    /// <item>Key: length.</item>
    /// <item>Value: number of words of that length.</item>
    /// </list>
    /// </summary>
    private static readonly SortedDictionary<int, int> _wordCountByLength = new();

    /// <summary>
    /// Distribution of the number of words by initial letter.
    /// <list type="table">
    /// <item>Key: initial letter.</item>
    /// <item>Value: number of words starting with that letter.</item>
    /// </list>
    /// </summary>
    private static readonly SortedDictionary<char, int> _wordCountByLetter = new();

    #endregion Fields

    #region Dictionary Initialization

    /// <summary>
    /// Initializes the <see cref="CustomDictionary"/> by loading words from a text file
    /// corresponding to the language specified in <see cref="Language.LanguageCode"/>.
    /// </summary>
    public static void Initialize()
    {
        ClearDataStructures();

        var tempWordsByLength = new SortedDictionary<int, List<string>>();
        var tempWordsByLetter = new SortedDictionary<char, List<string>>();

        LoadWordsFromFile(tempWordsByLength, tempWordsByLetter);
        ConvertListsToArrays(tempWordsByLength, tempWordsByLetter);
        SortLists();
    }

    /// <summary>
    /// Resets the internal data structures before a new load.
    /// </summary>
    private static void ClearDataStructures()
    {
        _wordCountByLength.Clear();
        _wordCountByLetter.Clear();
        _wordsByLength.Clear();
        _wordsByLetter.Clear();
    }

    /// <summary>
    /// Loads words from the file corresponding to the language, updating the temporary lists and counts.
    /// </summary>
    /// <param name="tempWordsByLength">Temporary dictionary of words by length.</param>
    /// <param name="tempWordsByLetter">Temporary dictionary of words by initial letter.</param>
    private static void LoadWordsFromFile(SortedDictionary<int, List<string>> tempWordsByLength,
                                          SortedDictionary<char, List<string>> tempWordsByLetter)
    {
        StreamReader? sReader = null;
        string file = "data/PossibleWords_" + Language.LanguageCode + ".txt";

        try
        {
            sReader = new StreamReader(file);
            string? line;

            while ((line = sReader.ReadLine()) != null)
            {
                line = line.Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    AddWordToStructures(line, tempWordsByLength, tempWordsByLetter);
                }
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("The file was not found: " + file);
            Console.WriteLine(e.Message);
        }
        catch (IOException e)
        {
            Console.WriteLine("Error reading the file.");
            Console.WriteLine(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while reading the file.");
            Console.WriteLine(e.Message);
        }
        finally
        {
            sReader?.Close();
        }
    }

    /// <summary>
    /// Adds a word to the temporary structures and updates the counters.
    /// </summary>
    /// <param name="word">Word to add.</param>
    /// <param name="tempWordsByLength">Temporary dictionary of words by length.</param>
    /// <param name="tempWordsByLetter">Temporary dictionary of words by initial letter.</param>
    private static void AddWordToStructures(string word,
                                            SortedDictionary<int, List<string>> tempWordsByLength,
                                            SortedDictionary<char, List<string>> tempWordsByLetter)
    {
        int length = word.Length;
        char initial = word[0];

        if (!tempWordsByLength.ContainsKey(length))
        {
            tempWordsByLength[length] = new List<string>();
        }
        tempWordsByLength[length].Add(word);

        if (!tempWordsByLetter.ContainsKey(initial))
        {
            tempWordsByLetter[initial] = new List<string>();
        }
        tempWordsByLetter[initial].Add(word);

        if (!_wordCountByLength.TryAdd(length, 1))
        {
            _wordCountByLength[length]++;
        }

        if (!_wordCountByLetter.TryAdd(initial, 1))
        {
            _wordCountByLetter[initial]++;
        }
    }

    /// <summary>
    /// Converts the collected word lists to arrays and assigns them to the final dictionaries.
    /// </summary>
    /// <param name="tempWordsByLength">Temporary dictionary of words by length.</param>
    /// <param name="tempWordsByLetter">Temporary dictionary of words by letter.</param>
    private static void ConvertListsToArrays(SortedDictionary<int, List<string>> tempWordsByLength,
                                             SortedDictionary<char, List<string>> tempWordsByLetter)
    {
        foreach (var kvp in tempWordsByLength)
        {
            _wordsByLength.Add(kvp.Key, kvp.Value.ToArray());
        }

        foreach (var kvp in tempWordsByLetter)
        {
            _wordsByLetter.Add(kvp.Key, kvp.Value.ToArray());
        }
    }

    /// <summary>
    /// Sorts the word lists.
    /// </summary>
    private static void SortLists()
    {
        foreach (var words in _wordsByLength.Select(kvp => kvp.Value))
        {
            QuickSort(words, 0, words.Length - 1);
        }
        foreach (var words in _wordsByLetter.Select(kvp => kvp.Value))
        {
            QuickSort(words, 0, words.Length - 1);
        }
    }

    #endregion Dictionary Initialization

    #region Word Search

    /// <summary>
    /// Checks if a word is present in the dictionary.
    /// The search uses the most relevant index (by length or by letter).
    /// </summary>
    /// <param name="word">Word to search for.</param>
    /// <returns><c>true</c> if the word is present, otherwise <c>false</c></returns>
    public static bool Contains(string word)
    {
        if (_wordCountByLength.ContainsKey(word.Length) && _wordCountByLetter.ContainsKey(word[0]))
        {
            int countByLength = _wordCountByLength[word.Length];
            int countByLetter = _wordCountByLetter[word[0]];

            if (countByLength <= countByLetter)
            {
                return BinarySearch(_wordsByLength[word.Length], word, 0, countByLength - 1);
            }
            else
            {
                return BinarySearch(_wordsByLetter[word[0]], word, 0, countByLetter - 1);
            }
        }

        return false;
    }

    /// <summary>
    /// Performs a recursive binary search to find an element in a sorted array of strings.
    /// </summary>
    /// <param name="array">Sorted array to search in.</param>
    /// <param name="word">Word to search for.</param>
    /// <param name="start">Start index of the search.</param>
    /// <param name="end">End index of the search.</param>
    /// <returns><c>true</c> if the word is in the array, otherwise <c>false</c>.</returns>
    private static bool BinarySearch(string[] array, string word, int start, int end)
    {
        if (start > end)
        {
            return false;
        }

        int middle = (start + end) / 2;

        if (array[middle] == word)
        {
            return true;
        }

        if (string.Compare(array[middle], word, StringComparison.Ordinal) < 0)
        {
            return BinarySearch(array, word, middle + 1, end);
        }
        else
        {
            return BinarySearch(array, word, start, middle - 1);
        }
    }

    #endregion Word Search

    #region Sorting

    /// <summary>
    /// QuickSort for an array of strings.
    /// </summary>
    /// <param name="array">Array to sort.</param>
    /// <param name="start">Start index of the sort.</param>
    /// <param name="end">End index of the sort.</param>
    public static void QuickSort(string[] array, int start, int end)
    {
        if (start < end)
        {
            int pivot = (start + end) / 2;
            pivot = Partition(array, start, end, pivot);
            QuickSort(array, start, pivot - 1);
            QuickSort(array, pivot + 1, end);
        }
    }

    /// <summary>
    /// Partitions the array using the pivot element.
    /// </summary>
    /// <param name="array">Array to partition.</param>
    /// <param name="start">Start index of the partition.</param>
    /// <param name="end">End index of the partition.</param>
    /// <param name="pivot">Index of the pivot element.</param>
    /// <returns>Index of the new position of the pivot after partitioning.</returns>
    private static int Partition(string[] array, int start, int end, int pivot)
    {
        string temp = array[pivot];
        array[pivot] = array[end];
        array[end] = temp;

        int j = start;

        for (int i = start; i < end; i++)
        {
            if (string.Compare(array[i], array[end], StringComparison.Ordinal) <= 0)
            {
                temp = array[j];
                array[j] = array[i];
                array[i] = temp;
                j++;
            }
        }
        temp = array[j];
        array[j] = array[end];
        array[end] = temp;

        return j;
    }

    #endregion Sorting
}