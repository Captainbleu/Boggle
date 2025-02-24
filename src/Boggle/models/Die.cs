namespace Boggle.Models;

/// <summary>
/// Represents a letter die.
/// </summary>
public class Die
{
    #region Fields

    /// <summary>
    /// List of die faces.
    /// <list type="table">
    /// <item>Key: the letter on the face.</item>
    /// <item>Value: the score of that letter.</item>
    /// </list>
    /// </summary>
    private readonly char[] _faces = new char[6];

    /// <summary>
    /// Top face of the die displayed on the board.
    /// </summary>
    private char _visibleFace;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Instantiates a new <see cref="Die"/> with faces randomly generated based on probabilities.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    public Die(Random random)
    {
        for (var i = 0; i < 6; i++)
        {
            _faces[i] = Language.GetLetterByProbability(random.Next(0, 100));
        }

        _visibleFace = _faces[random.Next(0, 6)];
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the visible face of the die.
    /// </summary>
    public char VisibleFace
    {
        get { return _visibleFace; }
    }

    /// <summary>
    /// Gets the points associated with the visible face of the die.
    /// </summary>
    public int Points
    {
        get { return Language.PointsPerLetter[_visibleFace]; }
    }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Rolls the die to get a random visible face.
    /// </summary>
    /// <param name="random">Random number generator.</param>
    public void Roll(Random random)
    {
        _visibleFace = _faces[random.Next(0, 6)];
    }

    /// <summary>
    /// Returns a string describing the die.
    /// </summary>
    /// <returns>Description of the die by the letters on each of its faces.</returns>
    public override string ToString()
    {
        StringBuilder bld = new("The die consists of the faces:");
        foreach (char letter in _faces)
        {
            bld.Append(' ').Append(letter);
        }

        return bld.ToString();
    }

    #endregion Methods
}