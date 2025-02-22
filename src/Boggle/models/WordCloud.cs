using KnowledgePicker.WordCloud;
using KnowledgePicker.WordCloud.Coloring;
using KnowledgePicker.WordCloud.Drawing;
using KnowledgePicker.WordCloud.Layouts;
using KnowledgePicker.WordCloud.Primitives;
using KnowledgePicker.WordCloud.Sizers;
using SkiaSharp;

namespace Boggle.models;

/// <summary>
/// Manages all aspects of the word cloud.
/// </summary>
public static class WordCloud
{
    /// <summary>
    /// Generates a word cloud from a dictionary of words and their frequencies.
    /// </summary>
    /// <param name="words">Dictionary containing the words and their frequencies.</param>
    /// <param name="name">Name of the player.</param>
    public static void GenerateWordCloud(Dictionary<string, int> words, string name)
    {
        double divisor = 1.1;

        IEnumerable<WordCloudEntry> wordEntries = words.SelectMany(p => Enumerable.Repeat(new WordCloudEntry(p.Key, (int)Math.Ceiling((double)p.Value / divisor)), p.Value));

        var wordCloud = new WordCloudInput(wordEntries)
        {
            Width = 1024,
            Height = 720,
            MinFontSize = 12,
            MaxFontSize = 32
        };

        var sizer = new LogSizer(wordCloud);
        using var engine = new SkGraphicEngine(sizer, wordCloud);
        var layout = new SpiralLayout(wordCloud);
        var colorizer = new RandomColorizer();
        var wcg = new WordCloudGenerator<SKBitmap>(wordCloud, engine, layout, colorizer);

        wcg.Arrange();

        using var final = new SKBitmap(wordCloud.Width, wordCloud.Height);
        using var canvas = new SKCanvas(final);

        canvas.Clear(SKColors.White);
        using var bitmap = wcg.Draw();
        canvas.DrawBitmap(bitmap, 0, 0);

        using var data = final.Encode(SKEncodedImageFormat.Png, 100);
        using var writer = File.Create("word_cloud_" + name + ".png");
        data.SaveTo(writer);
    }
}