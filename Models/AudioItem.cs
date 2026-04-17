namespace TGPSound.Models;

public class AudioItem
{
    // for Dezzer API
    public string? PreviewUrl { get; set; } = null;
    public string Duration { get; set; } = "";
    public string VideoId { get; set; } = "";
    public string Author { get; set; } = "";
    public string Title { get; set; } = "";
    public string Thumbnail { get; set; } = "";
}