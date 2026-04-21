using TGPSound.Ui;

namespace TGPSound.Common
{
    public static class AppStateExtensions
    {
        public static void SetMetadata(
            this AppState state,
            string title,
            string artist,
            string duration
        )
        {
            state.CurrentMetadata.Title = title;
            state.CurrentMetadata.Artist = artist;
            state.CurrentMetadata.Duration = duration;
        }

        public static void SetStreamingUrl(this AppState state, string url)
        {
            state.CurrentMetadata.Url = url;
        }

        public static void NavigateTo(this AppState state, Screen screen)
        {
            state.CurrentScreen = screen;
        }
    }
}
