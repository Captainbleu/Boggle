using System.Diagnostics;
using ConsoleAppVisuals;
using ConsoleAppVisuals.PassiveElements;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.AnimatedElements;

namespace Boggle;

/// <summary>
/// The main class of the game.
/// </summary>
public static class Game
{
    #region Fields

    /// <summary>
    /// Stopwatch for a player's turn.
    /// </summary>
    private static Stopwatch PlayerStopwatch = new Stopwatch();

    /// <summary>
    /// Time for a turn in milliseconds.
    /// </summary>
    private static int turnTime;

    /// <summary>
    /// Total number of turns per player.
    /// </summary>
    private static int totalTurns;

    /// <summary>
    /// List of players.
    /// </summary>
    private static List<Player> playerList = new List<Player>();

    /// <summary>
    /// Board size.
    /// </summary>
    private static int boardSize;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the list of players.
    /// </summary>
    public static List<Player> PlayerList
    {
        get { return playerList; }
    }

    #endregion Properties

    #region Main

    /// <summary>
    /// Entry point of the game application.
    /// </summary>
    public static void Main()
    {
        DisplayInstructions();

        InitializeGame();

        GameLoop();

        EndGame();
    }

    #endregion Main

    #region Initialization methods

    /// <summary>
    /// Initializes the game by setting up language, players, board, and game parameters.
    /// </summary>
    private static void InitializeGame()
    {
        InitializeLanguage();
        InitializePlayers();
        InitializeBoard();
        InitializeGameParameters();
    }

    /// <summary>
    /// Prompts the user to choose the game language and initializes the corresponding dictionary.
    /// </summary>
    private static void InitializeLanguage()
    {
        Console.WriteLine("Let the game begin!");
        Console.WriteLine("Choose the game language (e.g., FR, EN)");
        string input = Console.ReadLine();
        Language.Initialize(input);
    }

    /// <summary>
    /// Prompts the user to choose the board size and initializes the board.
    /// </summary>
    private static void InitializeBoard()
    {
        Console.WriteLine("It's time to choose the size of your board (e.g., 4 for a 4x4)");
        boardSize = Convert.ToInt32(Console.ReadLine());
        Board.Initialize(boardSize);
    }

    /// <summary>
    /// Prompts the user to set the number of turns and the time per turn, and initializes these parameters.
    /// </summary>
    private static void InitializeGameParameters()
    {
        Console.WriteLine("Number of turns per player (between 1 and 10):");
        totalTurns = Convert.ToInt32(Console.ReadLine());
        if (!IsValueInRange(totalTurns, 1, 10))
        {
            totalTurns = SetValueInRange(totalTurns, 1, 10);
        }

        Console.WriteLine("Time per turn in minutes (between 1 and 5):");
        int turnTimeMinutes = Convert.ToInt32(Console.ReadLine());
        if (!IsValueInRange(turnTimeMinutes, 1, 5))
        {
            turnTimeMinutes = SetValueInRange(turnTimeMinutes, 1, 5);
        }
        turnTime = turnTimeMinutes * 60_000;
    }

    /// <summary>
    /// Prompts the user to enter the number of players and their names, and initializes the player list.
    /// </summary>
    private static void InitializePlayers()
    {
        Console.WriteLine("How many players will participate? (at least 2)");
        int numPlayers = Convert.ToInt32(Console.ReadLine());
        if (numPlayers < 1)
        {
            Console.WriteLine("Incorrect number of players, defaulting to 2 players.");
            numPlayers = 2;
        }

        for (int i = 1; i <= numPlayers; i++)
        {
            Console.WriteLine("Enter the name of player " + i + ":");
            string playerName = Console.ReadLine();
            playerList.Add(new Player(playerName));
        }
    }

    #endregion Initialization methods

    #region Display methods

    /// <summary>
    /// Displays the game instructions to the players.
    /// </summary>
    private static void DisplayInstructions()
    {
        Console.WriteLine("In this game, you will need to find words on a board that will be given at the beginning of each turn.");
        Console.WriteLine("The one who finds the most words, but also the longest or with improbable letters, will win.");
    }

    /// <summary>
    /// Handles the end of the game by displaying final scores and word clouds.
    /// </summary>
    private static void EndGame()
    {
        Console.WriteLine("The game is over, well played!");
        DisplayScores();
        DisplayWordCloud();
    }

    /// <summary>
    /// Displays the final scores of all players.
    /// </summary>
    private static void DisplayScores()
    {
        Console.WriteLine("Final scores:");
        foreach (var player in playerList)
        {
            Console.WriteLine("Player " + player.Name + ": " + player.Score + " points");
        }
    }

    /// <summary>
    /// Displays the word cloud for each player.
    /// </summary>
    public static void DisplayWordCloud()
    {
        foreach(Player player in playerList)
        {
            Console.WriteLine(player.Name + ":");
            WordCloud.GenerateWordCloud(player.Words, player.Name);
            Console.WriteLine("Word cloud downloaded!");
        }
    }

    #endregion Display methods

    #region Playing methods

    /// <summary>
    /// Runs the main game loop, iterating through each turn for all players.
    /// </summary>
    private static void GameLoop()
    {
        Console.WriteLine("Game start!");
        for (int t = 0; t < totalTurns; t++)
        {
            foreach (var player in playerList)
            {
                PlayerLoop(player);
            }
        }
    }

    /// <summary>
    /// Handles a single player's turn, allowing them to find words within the time limit.
    /// </summary>
    /// <param name="player">Player whose turn it is.</param>
    private static void PlayerLoop(Player player)
    {
        Console.WriteLine("It's " + player.Name + "'s turn!");
        Board.Launch();
        Console.WriteLine(Board.ToString());
        PlayerStopwatch.Restart();

        while (IsTimeInTurnInterval(PlayerStopwatch.ElapsedMilliseconds))
        {
            Console.WriteLine("Find a word (or press Enter to end the turn):");
            string testWord = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(testWord))
                break;

            if (Board.TestWord(testWord) && IsTimeInTurnInterval(PlayerStopwatch.ElapsedMilliseconds))
            {
                player.AddWord(testWord);
                Console.WriteLine("Word accepted! " + player.Name + "'s score: " + player.Score);
            }
        }

        PlayerStopwatch.Stop();
        Console.WriteLine("Time's up! End of " + player.Name + "'s turn!");
    }

    #endregion Playing methods

    #region Utility methods

    /// <summary>
    /// Checks if the elapsed time is less than or equal to the turn time.
    /// </summary>
    /// <param name="time">Elapsed time in milliseconds.</param>
    /// <returns>Returns <c>true</c> if the time is less than or equal to <see cref="turnTime"/>, otherwise <c>false</c>.</returns>
    private static bool IsTimeInTurnInterval(long time)
    {
        return time <= turnTime;
    }

    /// <summary>
    /// Checks if the entered values are within the acceptable range.
    /// </summary>
    /// <param name="value">The value to test</param>
    /// <param name="minValue">Minimum accepted value</param>
    /// <param name="maxValue">Maximum accepted value</param>
    /// <returns>Returns true if the value is within the min and max limits</returns>
    private static bool IsValueInRange(int value, int minValue, int maxValue)
    {
        return (value >= minValue && value <= maxValue);
    }

    /// <summary>
    /// Sets a value within the specified range if it is out of bounds.
    /// </summary>
    /// <param name="value">The value to set if it is not correct.</param>
    /// <param name="minValue">Minimum accepted value.</param>
    /// <param name="maxValue">Maximum accepted value.</param>
    /// <returns>The corrected value.</returns>
    private static int SetValueInRange(int value, int minValue, int maxValue)
    {
        if (value < minValue)
        {
            Console.WriteLine("The value being too low has been set to: " + minValue);
            return minValue;
        }
        if (value > maxValue)
        {
            Console.WriteLine("The value being too high has been set to: " + maxValue);
            return maxValue;
        }
        return value;
    }

    #endregion Utility methods
}