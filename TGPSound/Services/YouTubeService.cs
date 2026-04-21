using System.Net.Http.Headers;
using TGPSound.Common;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace TGPSound.Services;
internal class YouTubeService
{
    private readonly YoutubeClient youtube = new();

    public async Task<Video> GetVideo(string videoId) =>
        await youtube.Videos.GetAsync(videoId);

    public async Task<IReadOnlyList<VideoSearchResult?>> SearchVideos(string query) =>
        await youtube.Search.GetVideosAsync(query);

    public async Task<StreamManifest?> GetStream(string videoId) =>
        await youtube.Videos.Streams.GetManifestAsync(videoId);
}
