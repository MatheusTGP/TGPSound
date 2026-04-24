using System.Net.Http.Json;
using TGPSound.Services.Responses;

namespace TGPSound.Services;

// Raw APIs services with original data models, not the one used in the app, so we can easily change API without changing the app logic
public static class APIsServices
{
    // To get 30s previews streamings
    public static async Task<DeezerSearchResult?> DeezerSearchTrack(string query)
    {
        return await Http.Client.GetFromJsonAsync<DeezerSearchResult>($"https://api.deezer.com/search/track?q={Uri.EscapeDataString(query)}");
    }

    // Litte speed search
    public static async Task<List<PaxsenixSearchResult>?> PaxsenixYouTubeSearch(string query)
    {
        return await Http.Client.GetFromJsonAsync<List<PaxsenixSearchResult>>($"https://lyrics.paxsenix.org/youtube/search?q={Uri.EscapeDataString(query)}");
    }

    // Speed Search!
    public static async Task<PipedSearchResult?> PipedYouTubeSearch(string query)
    {
        return await Http.Client.GetFromJsonAsync<PipedSearchResult?>($"https://api.piped.private.coffee/search?q={Uri.EscapeDataString(query)}&filter=music_songs");
    }

    // Basic plan lyrics search, with no time sync (contains), but it's fast and it works for most songs
    public static async Task<List<LrcLyricsItem>?> GetLrclibLyrics(string trackName, string artistName)
    {
        return await Http.Client.GetFromJsonAsync<List<LrcLyricsItem>>($"https://lrclib.net/api/search?track_name={Uri.EscapeDataString(trackName)}&artist_name={Uri.EscapeDataString(artistName)}");
    }
}
