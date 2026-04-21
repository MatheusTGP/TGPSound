using System.Text.Json.Serialization;

namespace TGPSound.Services.Models;

public class PipedSearchResult
{
    public List<PipedTrack> Items { get; set; } = [];
}

public class PipedTrack
{
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("uploaderName")]
    public string UploaderName { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public int Duration { get; set; } = 0;
}
