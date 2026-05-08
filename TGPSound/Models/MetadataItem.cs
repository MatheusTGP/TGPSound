namespace TGPSound.Models;

public class MetadataItem
{
    // current YouTube video ID
    public string VideoId { get; set; } = "";
    public string Title { get; private set; } = "";
    public string Artist { get; private set; } = "";
    public string Duration { get; private set; } = "";
    public string Url { get; private set; } = "";
    public string? PlainLyrics { get; private set; }
    public string? ThumbnailPath { get; private set; }
    public bool IsPlaying { get; set; } = false;

    public void Update(string title, string artist, string duration, string url)
    {
        Title = title;
        Artist = artist;
        Duration = duration;
        Url = url;
    }

    public void SetLyrics(string? lyrics)
        => PlainLyrics = lyrics;

    public void SetThumbnail(string? path)
        => ThumbnailPath = path;
}