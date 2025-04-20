using System.Diagnostics;
using ConsoleAppVisuals;
using ConsoleAppVisuals.AnimatedElements;
using ConsoleAppVisuals.Enums;
using ConsoleAppVisuals.InteractiveElements;
using ConsoleAppVisuals.PassiveElements;

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
    private static readonly Stopwatch playerStopwatch = new();

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
    private static readonly List<Player> playerList = [];

    /// <summary>
    /// Board size.
    /// </summary>
    private static int boardSize;

    /// <summary>
    /// Text displaying the scores.
    /// </summary>
    private static readonly EmbedText scoreText = new(
        ["Current scores:"],
        placement: Placement.TopRight
    );

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
        Console.OutputEncoding = Encoding.UTF8;
        Window.Open();

        Title title = new("Boggle");
        Header header = new(
            "By Eliott ROUSSILLE and François TEYNIER",
            "POO Project 2024-2025",
            "TD E"
        );
        Footer footer = new("[ESC] Quit", "[\u2191] Up [\u2193] Down", "[ENTER] Select");

        Window.AddElement(title, header, footer);
        Window.Render();
    }

    /// <summary>
    /// Initializes the game by setting up language, players, board, and game parameters.
    /// </summary>
    private static void InitializeGame()
    {
        Text beginText = new(["Let the game begin!"]);
        Window.AddElement(beginText);
        Window.Render();
        Thread.Sleep(1000);
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
        string[] choices = ["French", "English"];
        ScrollingMenu languageMenu = new("Choose the game language:", choices: choices);
        Window.AddElement(languageMenu);
        Window.ActivateElement(languageMenu);

        var languageResponse = languageMenu.GetResponse();
        switch (languageResponse?.Status)
        {
            case Status.Selected:
                Language.Initialize(choices[languageResponse.Value]);
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
    /// Prompts the user (via IntSelector) to choose the board size and initializes the board.
    /// </summary>
    private static void InitializeBoard()
    {
        IntSelector boardInput = new(
            "Enter the size of your board (e.g., 4 for a 4x4):",
            4,
            9,
            4,
            1
        );
        Window.AddElement(boardInput);
        Window.ActivateElement(boardInput);

        var boardSizeResponse = boardInput.GetResponse();
        if (boardSizeResponse?.Status == Status.Selected)
        {
            boardSize = boardSizeResponse.Value;
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
    /// Prompts the user (via IntSelector) to set the number of turns and time per turn.
    /// </summary>
    private static void InitializeGameParameters()
    {
        IntSelector turnsInput = new("Number of turns per player:", 1, 10, 3, 1);
        Window.AddElement(turnsInput);
        Window.ActivateElement(turnsInput);

        var turnNumberResponse = turnsInput.GetResponse();
        if (turnNumberResponse?.Status == Status.Selected)
        {
            totalTurns = turnNumberResponse.Value;
        }
        else
        {
            totalTurns = 3;
        }

        Window.RemoveElement(turnsInput);
        Window.Render();

        IntSelector timeInput = new("Time per turn in minutes:", 1, 5, 1, 1);
        Window.AddElement(timeInput);
        Window.ActivateElement(timeInput);

        var turnTimeResponse = timeInput.GetResponse();
        if (turnTimeResponse?.Status == Status.Selected)
        {
            turnTime = turnTimeResponse.Value * 60_000;
        }
        else
        {
            turnTime = 3 * 60_000;
        }

        Window.RemoveElement(timeInput);
        Window.Render();
    }

    /// <summary>
    /// Prompts the user (via IntSelector, Prompt and loop) for the number of players and their names.
    /// </summary>
    private static void InitializePlayers()
    {
        IntSelector playerNumberInput = new("How many players will participate?", 2, 10, 2, 1);
        Window.AddElement(playerNumberInput);
        Window.ActivateElement(playerNumberInput);

        var numPlayers = playerNumberInput.GetResponse()?.Value;

        Window.RemoveElement(playerNumberInput);
        Window.Render();

        for (int i = 1; i <= numPlayers; i++)
        {
            while (true)
            {
                Prompt nameInput = new($"Enter the name of player {i}:");
                Window.AddElement(nameInput);
                Window.ActivateElement(nameInput);
                var nameResponse = nameInput.GetResponse();
                if (nameResponse?.Status == Status.Selected)
                {
                    var player = Player.TryAddPlayer(nameResponse.Value, out string errorMessage);

                    if (player is not null)
                    {
                        playerList.Add(player);
                        Window.RemoveElement(nameInput);
                        Window.Render();
                        break;
                    }
                    Text errorText = new([errorMessage]);
                    Window.AddElement(errorText);
                    Window.Render();
                    Thread.Sleep(1000);
                    Window.RemoveElement(errorText);
                    Window.RemoveElement(nameInput);
                    Window.Render();
                }
            }
        }

        List<string> scores = ["Current scores:"];

        foreach (var player in playerList)
        {
            scores.Add($"Player {player.Name}: {player.Score} points");
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
        EmbedText instructions = new(
            [
                "In this game, you must find words on the board given at the beginning of each turn.",
                "Whoever finds the most words, especially the longest and with unusual letters, wins!",
            ]
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
        List<string> scores = ["Current scores:"];

        foreach (var player in playerList)
        {
            scores.Add($"Player {player.Name}: {player.Score} points");
        }

        scoreText.UpdateLines(scores);
        Window.Render();
    }

    /// <summary>
    /// Handles the end of the game by displaying final scores and word clouds.
    /// </summary>
    private static void EndGame()
    {
        EmbedText endText = new(["The game is over, well played!"]);
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

        List<string> finalScores = ["Final scores:"];
        int max = int.MinValue;
        string winner = "";
        foreach (var player in playerList)
        {
            finalScores.Add($"Player {player.Name}: {player.Score} points!");
            if (player.Score > max)
            {
                max = player.Score;
                winner = player.Name;
            }
        }

        finalScores.Add($"Congratulations to the winner: {winner} with {max} points!");
        finalScores.Add("Thanks for playing!");
        EmbedText finalScoreText = new(finalScores);
        Window.AddElement(finalScoreText);
        Window.Render();
    }

    /// <summary>
    /// Displays the word cloud for each player.
    /// </summary>
    private static void DisplayWordCloud()
    {
        List<string> wordCloudsText = [];
        foreach (var player in playerList)
        {
            WordCloud.GenerateWordCloud(player.FoundWords, player.Name);
            wordCloudsText.Add($"{player.Name}: Word cloud downloaded!");
        }

        EmbedText wordClouds = new(wordCloudsText, placement: Placement.TopRight);
        Window.AddElement(wordClouds);
        Window.Render();
    }

    /// <summary>
    /// A small helper to pause before closing the window.
    /// </summary>
    private static void WaitBeforeClose()
    {
        FakeLoadingBar fakeLoadingBar = new(
            "Closing the game...",
            processDuration: 15000,
            additionalDuration: 5000
        );
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
        FakeLoadingBar fakeLoadingBar = new(
            "Game start!",
            processDuration: 2000,
            additionalDuration: 1000
        );
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
        EmbedText playerTurn = new([$"It's {player.Name}'s turn!"]);
        Window.AddElement(playerTurn);
        Window.Render();

        Board.Launch();

        Matrix<char> board = new(Board.ToCharList(), Placement.TopCenter);
        Window.AddElement(board);
        Window.Render();

        playerStopwatch.Restart();

        while (IsTimeInTurnInterval(playerStopwatch.ElapsedMilliseconds))
        {
            Prompt wordInput = new("Find a word:");
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

                if (
                    Board.TestWord(testWord.Value, out string message)
                    && IsTimeInTurnInterval(playerStopwatch.ElapsedMilliseconds)
                )
                {
                    player.AddWord(testWord.Value);
                    DisplayScores();
                }

                Text wordMessage = new([message]);
                Window.AddElement(wordMessage);
                Window.Render();
                Thread.Sleep(500);
                Window.RemoveElement(wordMessage);
                Window.Render();
            }
        }

        playerStopwatch.Stop();

        Window.RemoveElement(board);
        Window.RemoveElement(playerTurn);
        Window.Render();

        EmbedText playerTurnEnd = new([$"Time's up! End of {player.Name}'s turn!"]);
        Window.AddElement(playerTurnEnd);
        Window.Render();

        FakeLoadingBar fakeLoadingBar = new(processDuration: 2000, additionalDuration: 1000);
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
