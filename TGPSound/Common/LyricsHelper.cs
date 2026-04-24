namespace TGPSound.Common;
static class LyricsHelper
{
    public static string GetLyricView(string txt, int lineIndex, int windowSize)
    {
        var lines = txt.Split('\n');
        lineIndex = Math.Clamp(lineIndex, 0, Math.Max(0, lines.Length - windowSize));

        var visible = lines
            .Skip(lineIndex)
            .Take(windowSize);

        return string.Join('\n', visible);
    }

    public static int ScrollDown(int index, int distance, int totalLines, int windowSize)
    {
        return Math.Min(index + distance, Math.Max(0, totalLines - windowSize));
    }

    public static int ScrollUp(int index, int distance)
    {
        return Math.Max(index - distance, 0);
    }
}
