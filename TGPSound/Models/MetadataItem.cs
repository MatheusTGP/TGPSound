namespace TGPSound.Models;

public class MetadataItem
{
    // current YouTube video ID
    public string VideoId { get; set; } = "";
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public bool IsPlaying { get; set; } = false;
    // cache/thumb.png -> or null if not loaded yet
    public string? ThumbnailPath { get; set; } = null;
    public string? PlainLyrics { get; set; } = null;
    public string Duration { get; set; } = "";   
}