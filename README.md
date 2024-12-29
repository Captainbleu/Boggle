# Boggle

This console program provides a Boggle game within a user interface.

## Overview

Boggle is a word game where players attempt to find words in sequences of adjacent letters on a grid of dice. This implementation includes a graphical user interface using the `ConsoleAppVisuals` library.

## Features

- **Randomized Board Generation**: The board is generated with dice that have faces with letters based on language-specific probabilities.
- **Multiple Languages**: Supports both English and French languages.
- **Player Management**: Allows multiple players to participate, each with their own score and found words.
- **Word Validation**: Checks words against a custom dictionary to ensure they are valid.
- **Word Cloud Generation**: Generates a word cloud image for each player based on the words they found.

## Classes

- **Game**: Main class that initializes and runs the game.
- **Player**: Represents a player with a name, score, and found words.
- **Board**: Manages the game board and word validation.
- **Die**: Represents a letter die with faces and a visible face.
- **Language**: Handles language-specific settings and letter probabilities.
- **CustomDictionary**: Manages the dictionary of valid words.
- **WordCloud**: Generates word cloud images for players.

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- `ConsoleAppVisuals` library
- `KnowledgePicker.WordCloud` library
- `SkiaSharp` library

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/Captainbleu/Boggle.git
    cd Boggle
    ```

2. Restore the required packages:
    ```sh
    dotnet restore
    ```

3. Build the project:
    ```sh
    dotnet build
    ```

### Running the Game

To run the game, execute the following command:
```sh
  dotnet run --project Application
```