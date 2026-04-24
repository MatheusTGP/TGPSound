using System.Text.Json.Serialization;

namespace TGPSound.Services.Responses;

public class DeezerSearchResult
{
    [JsonPropertyName("data")]
    public List<DeezerTrack> Data { get; set; } = [];
}

public class DeezerTrack
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("readable")]
    public bool Readable { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("title_short")]
    public string TitleShort { get; set; } = string.Empty;

    [JsonPropertyName("title_version")]
    public string TitleVersion { get; set; } = string.Empty;

    [JsonPropertyName("isrc")]
    public string Isrc { get; set; } = string.Empty;

    [JsonPropertyName("link")]
    public string Link { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [JsonPropertyName("rank")]
    public int Rank { get; set; }

    [JsonPropertyName("explicit_lyrics")]
    public bool ExplicitLyrics { get; set; }

    [JsonPropertyName("explicit_content_lyrics")]
    public int ExplicitContentLyrics { get; set; }

    [JsonPropertyName("explicit_content_cover")]
    public int ExplicitContentCover { get; set; }

    [JsonPropertyName("preview")]
    public string Preview { get; set; } = string.Empty;

    [JsonPropertyName("md5_image")]
    public string Md5Image { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public DeezerArtist Artist { get; set; } = new();

    [JsonPropertyName("album")]
    public DeezerAlbum Album { get; set; } = new();

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class DeezerArtist
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("link")]
    public string Link { get; set; } = string.Empty;

    [JsonPropertyName("picture")]
    public string Picture { get; set; } = string.Empty;

    [JsonPropertyName("picture_small")]
    public string PictureSmall { get; set; } = string.Empty;

    [JsonPropertyName("picture_medium")]
    public string PictureMedium { get; set; } = string.Empty;

    [JsonPropertyName("picture_big")]
    public string PictureBig { get; set; } = string.Empty;

    [JsonPropertyName("picture_xl")]
    public string PictureXl { get; set; } = string.Empty;

    [JsonPropertyName("tracklist")]
    public string Tracklist { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class DeezerAlbum
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("cover")]
    public string Cover { get; set; } = string.Empty;

    [JsonPropertyName("cover_small")]
    public string CoverSmall { get; set; } = string.Empty;

    [JsonPropertyName("cover_medium")]
    public string CoverMedium { get; set; } = string.Empty;

    [JsonPropertyName("cover_big")]
    public string CoverBig { get; set; } = string.Empty;

    [JsonPropertyName("cover_xl")]
    public string CoverXl { get; set; } = string.Empty;

    [JsonPropertyName("md5_image")]
    public string Md5Image { get; set; } = string.Empty;

    [JsonPropertyName("tracklist")]
    public string Tracklist { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}