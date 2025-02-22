namespace Boggle.models;

/// <summary>
/// Represents a matrix of <see cref="Die"/>.
/// </summary>
public static class Board
{
    #region Fields

    private static readonly Random r = new();
    private static Die[,] _board;
    private static readonly List<string> _foundWords = new();

    #endregion Fields

    #region Initialization

    /// <summary>
    /// Initializes the board based on a size.
    /// Also updates the maximum occurrences of each letter according to the size.
    /// </summary>
    /// <param name="size">Size of the game grid</param>
    public static void Initialize(int size)
    {
        InitializeMatrix(size);
        UpdateMaxOccurrences(size);
        Launch();
    }

    /// <summary>
    /// Creates the board matrix with initialized dice.
    /// </summary>
    /// <param name="size">Size of the board</param>
    private static void InitializeMatrix(int size)
    {
        _board = new Die[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                _board[i, j] = new Die(r);
            }
        }
    }

    /// <summary>
    /// Updates the maximum occurrences of letters according to the size.
    /// The ratio is based on a standard 4x4 board.
    /// </summary>
    /// <param name="size">Size of the board</param>
    private static void UpdateMaxOccurrences(int size)
    {
        double ratio = (double)(size * size) / 16.0;

        for (int i = 0; i < size; i++)
        {
            char key = Language.MaxOccurrences.GetKeyAtIndex(i);
            double value = Math.Ceiling(Language.MaxOccurrences[key] * ratio);
            Language.MaxOccurrences[key] = (int)value;
        }
    }

    #endregion Initialization

    #region Board Generation

    /// <summary>
    /// Generates the board by rolling the dice and ensuring that letter occurrences
    /// respect the constraints. If necessary, re-rolls the dice until a valid board is obtained.
    /// </summary>
    /// <exception cref="Exception">If the board cannot be generated after many attempts.</exception>
    public static void Launch()
    {
        var occurrenceCounter = new SortedList<char, int>(Language.MaxOccurrences);
        FillBoardWithConstraints(occurrenceCounter);
        _foundWords.Clear();
    }

    /// <summary>
    /// Attempts to fill the board while respecting the maximum occurrences, re-rolls if necessary.
    /// </summary>
    /// <param name="occurrenceCounter">Available occurrences for each character.</param>
    private static void FillBoardWithConstraints(SortedList<char, int> occurrenceCounter)
    {
        int size = _board.GetLength(0);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GenerateValidFaceForCell(i, j, occurrenceCounter);
            }
        }
    }

    /// <summary>
    /// Generates a die face for a given cell, re-rolling if the generated letter is not allowed.
    /// </summary>
    /// <param name="x">X coordinate in the board.</param>
    /// <param name="y">Y coordinate in the board.</param>
    /// <param name="occurrenceCounter">Remaining occurrences for each letter.</param>
    private static void GenerateValidFaceForCell(int x, int y, SortedList<char, int> occurrenceCounter)
    {
        int errorCount = 0;
        int maxAttempts = 30;

        do
        {
            _board[x, y].Roll(r);

            if (occurrenceCounter[_board[x, y].VisibleFace] > 0)
            {
                occurrenceCounter[_board[x, y].VisibleFace]--;
                return;
            }

            errorCount++;
            if (errorCount >= 5)
            {
                _board[x, y] = new Die(r);
            }

            if (errorCount > maxAttempts)
            {
                throw new Exception("Board cannot be generated");
            }

        } while (true);
    }

    #endregion Board Generation

    #region Word Testing

    /// <summary>
    /// Checks if a word is valid and if so, adds it to the list of found words.
    /// </summary>
    /// <param name="word">Word to check.</param>
    /// <param name="message">Message to display to the user.</param>
    /// <returns><c>true</c> if the word is valid, otherwise <c>false</c>.</returns>
    public static bool TestWord(string word, out string message)
    {
        word = CleanWord(word);

        if (!IsWordLongEnough(word))
        {
            message = "The word must be at least two letters long.";
            return false;
        }
        if (!IsWordNotFound(word))
        {
            message = "The word has already been found.";
            return false;
        }
        if (!IsWordInBoard(word))
        {
            message = "The word is not in the board.";
            return false;
        }
        if (!IsWordInDictionary(word))
        {
            message = "The word is not in the dictionary.";
            return false;
        }

        _foundWords.Add(word);
        message = "Word accepted!";
        return true;
    }

    /// <summary>
    /// Cleans the word by removing special characters and converting it to uppercase.
    /// </summary>
    private static string CleanWord(string word)
    {
        return word.Trim(" ,?;.:/!§%*µ$£^¨<>&~#{([-|`_@)]=}+°".ToCharArray()).ToUpper();
    }

    /// <summary>
    /// Checks that the word has a minimum length (2).
    /// </summary>
    private static bool IsWordLongEnough(string word)
    {
        if (word.Length < 2)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks that the word has not already been found.
    /// </summary>
    private static bool IsWordNotFound(string word)
    {
        if (_foundWords.Contains(word))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks that the word is present in the board.
    /// </summary>
    private static bool IsWordInBoard(string word)
    {
        if (!Contains(word))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks that the word is in the dictionary.
    /// </summary>
    private static bool IsWordInDictionary(string word)
    {
        if (!CustomDictionary.Contains(word))
        {
            return false;
        }
        return true;
    }

    #endregion Word Testing

    #region Word Search in Board

    /// <summary>
    /// Checks if the word is present in the board.
    /// </summary>
    /// <param name="word">Word to check.</param>
    /// <returns><c>true</c> if the word is in the board, otherwise <c>false</c>.</returns>
    public static bool Contains(string word)
    {
        int size = _board.GetLength(0);
        bool[,] visited = new bool[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (_board[i, j].VisibleFace == word[0] && SearchWord(word, 1, i, j, visited))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Recursive search to check if a word can be formed from a given position.
    /// </summary>
    /// <param name="word">The word to search for.</param>
    /// <param name="index">Index of the current letter in the word.</param>
    /// <param name="x">Current X position in the board.</param>
    /// <param name="y">Current Y position in the board.</param>
    /// <param name="visited">Matrix of already used cells.</param>
    /// <returns><c>True</c> if the word can be formed, otherwise <c>False</c>.</returns>
    private static bool SearchWord(string word, int index, int x, int y, bool[,] visited)
    {
        if (index == word.Length)
        {
            return true;
        }

        visited[x, y] = true;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (DoesNextLetterMatch(word, index, x, y, dx, dy, visited))
                {
                    return true;
                }
            }
        }

        visited[x, y] = false;
        return false;
    }

    /// <summary>
    /// Checks if the next letter of the word can be found in the neighboring cell.
    /// </summary>
    private static bool DoesNextLetterMatch(string word, int index, int x, int y, int dx, int dy, bool[,] visited)
    {
        int nx = x + dx;
        int ny = y + dy;

        if ((dx != 0 || dy != 0) && AreCoordinatesValid(nx, ny) && !visited[nx, ny] && _board[nx, ny].VisibleFace == word[index] && SearchWord(word, index + 1, nx, ny, visited))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks that the coordinates are within the board limits.
    /// </summary>
    private static bool AreCoordinatesValid(int x, int y)
    {
        int size = _board.GetLength(0);
        return x >= 0 && x < size && y >= 0 && y < size;
    }

    #endregion Word Search in Board

    #region Display

    /// <summary>
    /// Returns the visible faces of the board as a string.
    /// </summary>
    /// <returns>string of the visible faces of the board</returns>
    public static string toString()
    {
        StringBuilder bld = new();
        int size = _board.GetLength(0);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                bld.Append(_board[i, j].VisibleFace).Append(' ');
            }
            bld.Append('\n');
        }

        return bld.ToString().TrimEnd();
    }

    /// <summary>
    /// Returns the visible faces of the board as a list of lists of characters.
    /// </summary>
    /// <returns>list of lists of the visible faces of the board</returns>
    public static List<List<char>> ToCharList()
    {
        int size = _board.GetLength(0);
        List<List<char>> list = new();

        for (int i = 0; i < size; i++)
        {
            List<char> row = new();
            for (int j = 0; j < size; j++)
            {
                row.Add(_board[i, j].VisibleFace);
            }
            list.Add(row);
        }

        return list;
    }

    #endregion Display
}