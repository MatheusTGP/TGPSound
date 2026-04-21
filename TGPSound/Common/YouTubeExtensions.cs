using TGPSound.Models;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace TGPSound.Common
{
    public static class YouTubeExtensions
    {
        public static VideoItem ToVideoItem(this Video video)
        {
            return new VideoItem
            {
                Id = video.Id,
                Title = video.Title,
                LikeCount = video.Engagement.LikeCount,
                ViewCount = video.Engagement.ViewCount,
                Thumbnail = video.Thumbnails[0].Url
            };
        }

        public static string GetBestAudioUrl(this StreamManifest manifest)
        {
            return manifest
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate()
                .Url;
        }

        public static string GetBestVideoUrl(this StreamManifest manifest)
        {
            return manifest
                .GetVideoOnlyStreams()
                .GetWithHighestBitrate()
                .Url;
        }
    }
}
