
using TGPSound.Ui;

namespace TGPSound.Common
{
    public static class StateExtensions
    {
        // set plain lyrics in PlayerMetadata
        public static void SetPlainLyrics(this AppState state, string? plainLyrics)
        {
            state.PlayerMetadata.SetLyrics(plainLyrics);
        }

        // navigate screen in ScreenManager
        public static void NavigateTo(this AppState state, Screen screen)
        {
            state.CurrentScreen = screen;
        }
    }
}
