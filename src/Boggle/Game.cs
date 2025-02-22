using System.Diagnostics;
using ConsoleAppVisuals;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.AnimatedElements;
using ConsoleAppVisuals.PassiveElements;
using ConsoleAppVisuals.Enums;

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
    private static Stopwatch playerStopwatch = new Stopwatch();

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

    /// <summary>
    /// Text displaying the scores.
    /// </summary>
    private static EmbedText scoreText = new EmbedText(new List<string> { "Current scores:" }, placement: Placement.TopRight);

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
        InitializeUI();
        DisplayInstructions();
        InitializeGame();
        GameLoop();
        EndGame();
    }

    #endregion Main

    #region Initialization methods

    /// <summary>
    /// Initializes the graphical user interface using ConsoleAppVisuals.
    /// </summary>
    private static void InitializeUI()
    {
        Window.Open();

        Title title = new Title("Boggle");
        Header header = new Header("By Eliott ROUSSILLE and François TEYNIER",
                                "POO Project 2024-2025",
                                "TD E");
        Footer footer = new Footer("[ESC] Quit",
                                "[Z | \u2191] Up, [S | \u2193] Down",
                                "[ENTER] Select");

        Window.AddElement(title, header, footer);
        Window.Render();
    }

    /// <summary>
    /// Initializes the game by setting up language, players, board, and game parameters.
    /// </summary>
    private static void InitializeGame()
    {
        Text beginText = new Text(new List<string> { "Let the game begin!" });
        Window.AddElement(beginText);
        Window.Render();
        System.Threading.Thread.Sleep(1000);
        Window.RemoveElement(beginText);
        Window.Render();
        InitializeLanguage();
        InitializePlayers();
        InitializeBoard();
        InitializeGameParameters();
    }

    /// <summary>
    /// Prompts the user (via a ScrollingMenu) to choose the game language and initializes the corresponding dictionary.
    /// </summary>
    private static void InitializeLanguage()
    {
        ScrollingMenu languageMenu = new ScrollingMenu(
            "Choose the game language:",
            choices: ["French", "English"]
        );
        Window.AddElement(languageMenu);
        Window.ActivateElement(languageMenu);

        var response = languageMenu.GetResponse();
        switch (response?.Status)
        {
            case Status.Selected:
                if (response?.Value == 0)
                {
                    Language.Initialize("FR");
                }
                else
                {
                    Language.Initialize("EN");
                }
                break;
            case Status.Deleted:
                break;
            case Status.Escaped:
                break;
        }

        Window.RemoveElement(languageMenu);
        Window.Render();
    }

    /// <summary>
    /// Prompts the user (via Prompt) to choose the board size and initializes the board.
    /// </summary>
    private static void InitializeBoard()
    {
        IntSelector boardInput = new IntSelector("Enter the size of your board (e.g., 4 for a 4x4):", 4, 9, 4, 1);
        Window.AddElement(boardInput);
        Window.ActivateElement(boardInput);

        var response = boardInput.GetResponse();
        if (response?.Status == Status.Selected)
        {
            boardSize = response.Value;
        }
        else
        {
            boardSize = 4;
        }

        Window.RemoveElement(boardInput);
        Window.Render();

        Board.Initialize(boardSize);
    }

    /// <summary>
    /// Prompts the user (via Prompt) to set the number of turns and time per turn.
    /// </summary>
    private static void InitializeGameParameters()
    {
        IntSelector turnsInput = new IntSelector("Number of turns per player:", 1, 10, 3, 1);
        Window.AddElement(turnsInput);
        Window.ActivateElement(turnsInput);

        var response = turnsInput.GetResponse();
        if (response?.Status == Status.Selected)
        {
            totalTurns = response.Value;
        }
        else
        {
            totalTurns = 3;
        }

        Window.RemoveElement(turnsInput);
        Window.Render();

        IntSelector timeInput = new IntSelector("Time per turn in minutes:", 1, 5, 1, 1);
        Window.AddElement(timeInput);
        Window.ActivateElement(timeInput);

        response = timeInput.GetResponse();
        if (response?.Status == Status.Selected)
        {
            turnTime = response.Value * 60_000;
        }
        else
        {
            turnTime = 3 * 60_000;
        }

        Window.RemoveElement(timeInput);
        Window.Render();
    }

    /// <summary>
    /// Prompts the user (via Prompt and loop) for the number of players and their names.
    /// </summary>
    private static void InitializePlayers()
    {
        Prompt playerNumberInput = new Prompt("How many players will participate? (at least 2)");
        Window.AddElement(playerNumberInput);
        Window.ActivateElement(playerNumberInput);

        var response = playerNumberInput.GetResponse();

        Window.RemoveElement(playerNumberInput);
        Window.Render();

        int numPlayers = Convert.ToInt32(response?.Value);
        if (numPlayers < 2)
        {
            numPlayers = 2;
        }

        for (int i = 1; i <= numPlayers; i++)
        {
            while (true)
            {
                Prompt nameInput = new Prompt($"Enter the name of player {i}:");
                Window.AddElement(nameInput);
                Window.ActivateElement(nameInput);
                response = nameInput.GetResponse();
                if (response?.Status == Status.Selected)
                {
                    if (Player.TryAddPlayer(response.Value, out string errorMessage))
                    {
                        Window.RemoveElement(nameInput);
                        Window.Render();
                        break;
                    }

                    Text errorText = new Text(new List<string> { errorMessage });
                    Window.AddElement(errorText);
                    Window.Render();
                    Thread.Sleep(1000);
                    Window.RemoveElement(errorText);
                    Window.RemoveElement(nameInput);
                    Window.Render();
                }
            }
        }

        List<string> scores = new List<string> { "Current scores:" };

        foreach (var player in playerList)
        {
            scores.Add("Player " + player.Name + ": " + player.Score + " points");
        }

        scoreText.UpdateLines(scores);
        Window.AddElement(scoreText);
        Window.Render();
    }

    #endregion Initialization methods

    #region Display methods

    /// <summary>
    /// Displays the game instructions to the players.
    /// </summary>
    private static void DisplayInstructions()
    {
        EmbedText instructions = new EmbedText(
            new List<string> { "In this game, you must find words on the board given at the beginning of each turn.", "Whoever finds the most words, especially the longest and with unusual letters, wins!" }
        );
        Window.AddElement(instructions);
        Window.Render();

        Window.RemoveElement(instructions);
        Window.Render();
    }

    /// <summary>
    /// Displays the current scores of all players.
    /// </summary>
    private static void DisplayScores()
    {
        List<string> scores = new List<string> { "Current scores:" };

        foreach (var player in playerList)
        {
            scores.Add("Player " + player.Name + ": " + player.Score + " points");
        }

        scoreText.UpdateLines(scores);
        Window.Render();
    }

    /// <summary>
    /// Handles the end of the game by displaying final scores and word clouds.
    /// </summary>
    private static void EndGame()
    {
        EmbedText endText = new EmbedText(new List<string> { "The game is over, well played!" });
        Window.AddElement(endText);
        Window.Render();
        DisplayFinalScores();
        DisplayWordCloud();
        WaitBeforeClose();
    }

    /// <summary>
    /// Displays the final scores of all players.
    /// </summary>
    private static void DisplayFinalScores()
    {
        Window.RemoveElement(scoreText);

        List<string> finalScores = new List<string> { "Final scores:" };
        int max = Int32.MinValue;
        string winner = "";
        foreach (var player in playerList)
        {
            finalScores.Add("Player " + player.Name + ": " + player.Score + " points");
            if (player.Score > max)
            {
                max = player.Score;
                winner = player.Name;
            }
        }

        finalScores.Add("Congratulations to the winner: " + winner + " with " + max + " points!");
        EmbedText finalScoreText = new EmbedText(finalScores);
        Window.AddElement(finalScoreText);
        Window.Render();
    }

    /// <summary>
    /// Displays the word cloud for each player.
    /// </summary>
    private static void DisplayWordCloud()
    {
        List<string> wordCloudsText = new List<string>();
        foreach (Player player in playerList)
        {
            WordCloud.GenerateWordCloud(player.FoundWords, player.Name);
            wordCloudsText.Add(player.Name + ": Word cloud downloaded!");
        }

        EmbedText wordClouds = new EmbedText(wordCloudsText, placement: Placement.TopRight);
        Window.AddElement(wordClouds);
        Window.Render();
    }

    /// <summary>
    /// A small helper to pause before closing the window.
    /// </summary>
    private static void WaitBeforeClose()
    {
        FakeLoadingBar fakeLoadingBar = new FakeLoadingBar("Closing the game...", processDuration: 20000, additionalDuration: 10000);
        Window.AddElement(fakeLoadingBar);
        Window.ActivateElement(fakeLoadingBar);
    }

    #endregion Display methods

    #region Playing methods

    /// <summary>
    /// Runs the main game loop, iterating through each turn for all players.
    /// </summary>
    private static void GameLoop()
    {
        FakeLoadingBar fakeLoadingBar = new FakeLoadingBar("Game start!", processDuration: 2000, additionalDuration: 1000);
        Window.AddElement(fakeLoadingBar);
        Window.ActivateElement(fakeLoadingBar);
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
        EmbedText playerTurn = new EmbedText(new List<string> { "It's " + player.Name + "'s turn!" });
        Window.AddElement(playerTurn);
        Window.Render();

        Board.Launch();

        Matrix<char> board = new Matrix<char>(Board.ToCharList(), Placement.TopCenter);
        Window.AddElement(board);
        Window.Render();

        playerStopwatch.Restart();

        string message;
        while (IsTimeInTurnInterval(playerStopwatch.ElapsedMilliseconds))
        {
            Prompt wordInput = new Prompt("Find a word:");
            Window.AddElement(wordInput);
            Window.ActivateElement(wordInput);
            var testWord = wordInput.GetResponse();
            Window.RemoveElement(wordInput);
            Window.Render();

            if (testWord?.Status == Status.Selected)
            {
                if (string.IsNullOrWhiteSpace(testWord.Value))
                {
                    break;
                }

                if (Board.TestWord(testWord.Value, out message) && IsTimeInTurnInterval(playerStopwatch.ElapsedMilliseconds))
                {
                    player.AddWord(testWord.Value);
                    DisplayScores();
                }

                Text wordMessage = new Text(new List<string> { message });
                Window.AddElement(wordMessage);
                Window.Render();
                System.Threading.Thread.Sleep(500);
                Window.RemoveElement(wordMessage);
                Window.Render();
            }
        }

        playerStopwatch.Stop();

        Window.RemoveElement(board);
        Window.RemoveElement(playerTurn);
        Window.Render();

        EmbedText playerTurnEnd = new EmbedText(new List<string> { "Time's up! End of " + player.Name + "'s turn!" });
        Window.AddElement(playerTurnEnd);
        Window.Render();

        FakeLoadingBar fakeLoadingBar = new FakeLoadingBar(processDuration: 2000, additionalDuration: 1000);
        Window.AddElement(fakeLoadingBar);
        Window.ActivateElement(fakeLoadingBar);

        Window.RemoveElement(playerTurnEnd);
        Window.Render();
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

    #endregion Utility methods
}