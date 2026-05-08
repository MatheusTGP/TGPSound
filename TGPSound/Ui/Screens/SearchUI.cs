using Spectre.Console;
using Spectre.Console.Rendering;
using TGPSound.Common;
using TGPSound.Services;
using TGPSound.Services.Responses;

namespace TGPSound.Ui.Screens;

internal class SearchUI(AppState state) : IScreen
{
    private const int MAX_SEARCH_RESULTS = 10;

    private readonly string DEFAULT_THUMB_PATH = Path.Combine("cache", "thumb.png");
    private readonly AppState appState = state;
    private readonly YouTubeService youtube = new();

    private List<PaxsenixSearchResult>? searchResults = [];
    private bool isSearching = false;
    public int selectedItemIndex = 0;

    public IRenderable Render()
    {
        return new Panel(
            new Rows(
                new FigletText("Search") { Color = Color.Aqua }.Centered(),
                new Markup("[AQUA]Search musics from YouTube and listen![/]").Centered(),
                new Rows(
                    new Markup("[blue]P[/] = Search"),
                    new Markup("[blue]K[/] = Toggle Keyboard"),
                    new Markup("[blue]V[/] = Clipboard Paste"),
                    new Markup("[blue]C[/] = Clear current selected input"),
                    new Markup("[blue]T[/] = Play selected item"),
                    new Markup("[blue]ENTER[/] = Save search")
                ),
                new Markup("\n"),
                new Grid()
                    .AddColumn(new GridColumn().Width(50))
                    .AddColumn(new GridColumn().Width(50))
                    .AddRow(
                        new Panel(
                            new Markup($"[aqua]{appState.InputBuffer}[/][grey]_[/]\n")
                        )
                        .Header("[bold white]INPUT-BOX[/]")
                        .Border(BoxBorder.Rounded)
                        .Expand(),
                        new Panel(
                            new Rows(
                                new Markup($"isTyping: [green]{appState.IsTyping}[/]"),
                                new Markup($"Selected: [aqua]{appState.CurrentInput}[/]")
                            )
                        )
                        .Header("[bold white]STATES[/]")
                        .Border(BoxBorder.Rounded)
                        .Expand()
                    ),
                RenderResults()
            )
        ).Expand();
    }
    private IRenderable RenderResults()
    {
        if (isSearching)
        {
            return new Markup($"[white]Searching for[/][bold blue] {appState.CurrentInput}[/]").Centered();
        }
        if (searchResults != null)
        {
            var rows = new List<IRenderable> { new Markup("Waiting your search").Centered() };
            foreach (var (music, index) in searchResults.Take(MAX_SEARCH_RESULTS + 1).Select((v, i) => (v, i)))
            {
                var selectedColor = selectedItemIndex == index
                    ? "blue"
                    : "white";

                try
                {
                    rows.Add(new Markup($"[gray]{index}[/] [bold {selectedColor}]{music?.Title.RemoveMarkup()}[/]"));
                    rows.Add(new Markup($"  [gray]{music?.Author}[/]"));
                    rows.Add(new Rule());
                }
                catch
                {
                    rows.Add(new Text($"{index}| {music?.Title} | {music?.Author}"));
                }
            }
            return new Rows(rows);
        }
        return new Markup("[gray]Not found results[/]").Centered();
    }

    private async Task BuildStreamAsync()
    {
        var video = searchResults?[selectedItemIndex];
        if (video != null)
        {
            // Navigate to player screen before fetching metadata to show loading state
            appState.NavigateTo(Screen.Player);
            var metadata = await youtube.GetStream(video.VideoId);
            if (metadata != null)
            {
                appState.PlayerMetadata.Update(
                    title: video.Title,
                    artist: video.Author,
                    duration: video.Duration,
                    url: metadata.GetBestAudioUrl()
                );
                // Cache thumbnail to avoid re-downloading it in the player screen
                if (!Directory.Exists(DEFAULT_THUMB_PATH)) Directory.CreateDirectory("cache");
                var bytes = await Http.Client.GetByteArrayAsync(video.Thumbnail);

                try {
                    _ = File.WriteAllBytesAsync(DEFAULT_THUMB_PATH, bytes);
                }
                finally {
                    appState.PlayerMetadata.SetThumbnail(DEFAULT_THUMB_PATH);
                }
            }
        }
    }

    private async Task SearchYouTubeAsync()
    {
        isSearching = true;
        try {
            var result = await APIsServices.PaxsenixYouTubeSearch(appState.CurrentInput);
            if (result == null) return;
            searchResults = result;
        }
        finally{
            isSearching = false;
        }
    }
    public async Task HandleActions(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Q:
                appState.CurrentScreen = Screen.Main;
                break;

            case ConsoleKey.V:
                var clipboardText = WinClipboardHelper.GetClipboardString();
                // Avoid pasting too long text or null
                if (clipboardText == null || clipboardText.Length > 512) return;
                appState.CurrentInput = clipboardText;
                break;

            case ConsoleKey.C:
                appState.CurrentInput = "";
                break;

            case ConsoleKey.P:
                if (appState.CurrentInput.Length == 0) return;
                _ = SearchYouTubeAsync();
                break;

            case ConsoleKey.T:
                _ = BuildStreamAsync();
                break;

            case ConsoleKey.K:
                appState.IsTyping = true;
                appState.InputBuffer = "";
                break;

            case ConsoleKey.UpArrow:
                if (selectedItemIndex != 0)
                {
                    selectedItemIndex--;
                    break;
                }
                selectedItemIndex = MAX_SEARCH_RESULTS;
                break;

            case ConsoleKey.DownArrow:
                if (selectedItemIndex != MAX_SEARCH_RESULTS)
                {
                    selectedItemIndex++;
                    break;
                }
                selectedItemIndex = 0;
                break;
        }
    }
}
