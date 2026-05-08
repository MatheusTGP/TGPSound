using Spectre.Console;
using Spectre.Console.Rendering;
using TGPSound.Common;
using TGPSound.Services;
using TGPSound.Services.Responses;

namespace TGPSound.Ui.Screens;

internal class SearchUI(AppState state) : IScreen
{
    private const int MAX_SEARCH_RESULTS = 10;

    private readonly string defaultThumbPath = Path.Combine("cache", "thumb.png");
    private readonly AppState _state = state;
    private readonly YouTubeService youtube = new();

    private List<PaxsenixSearchResult>? searchResults = [];
    private CancellationTokenSource? _cts;
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
                            new Markup($"[aqua]{_state.InputBuffer}[/][grey]_[/]\n")
                        )
                        .Header("[bold white]INPUT-BOX[/]")
                        .Border(BoxBorder.Rounded)
                        .Expand(),
                        new Panel(
                            new Rows(
                                new Markup($"isTyping: [green]{_state.IsTyping}[/]"),
                                new Markup($"Selected: [aqua]{_state.CurrentInput}[/]")
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
            return new Markup($"[white]Searching for[/][bold blue] {_state.CurrentInput}[/]").Centered();
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
            var metadata = await youtube.GetStream(video.VideoId);
            if (metadata != null)
            {
                BuildPlayerState(video, metadata.GetBestAudioUrl());
                await LoadThumbnailAsync(video.Thumbnail);
            }
        }
    }

    private void BuildPlayerState(PaxsenixSearchResult video, string streamUrl)
    {
        _state.NavigateTo(Screen.Player);
        _state.CurrentMetadata.Title = video.Title;
        _state.CurrentMetadata.Artist = video.Author;
        _state.CurrentMetadata.VideoId = video.VideoId;
        _state.CurrentMetadata.Duration = video.Duration;
        _state.CurrentMetadata.Url = streamUrl;
    }

    private async Task LoadThumbnailAsync(string imageUrl)
    {
        if (!Directory.Exists(defaultThumbPath))
        {
            Directory.CreateDirectory("cache");
        }

        var bytes = await Http.Client.GetByteArrayAsync(imageUrl);
        try
        {
            await File.WriteAllBytesAsync(defaultThumbPath, bytes);
        }
        finally
        {
            _state.CurrentMetadata.ThumbnailPath = defaultThumbPath;
        }
    }

    private async Task SearchYouTubeAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        isSearching = true;
        try
        {
            var result = await APIsServices.PaxsenixYouTubeSearch(_state.CurrentInput);
            if (token.IsCancellationRequested && result == null) return;
            searchResults = result;
        }
        finally
        {
            isSearching = false;
        }
    }
    public async Task HandleActions(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Q:
                _state.CurrentScreen = Screen.Main;
                break;

            case ConsoleKey.V:
                var clipboardText = WinClipboardHelper.GetClipboardString();
                // Avoid pasting too long text or null
                if (clipboardText == null || clipboardText.Length > 512) return;
                _state.CurrentInput = clipboardText;
                break;

            case ConsoleKey.C:
                _state.CurrentInput = "";
                break;

            case ConsoleKey.P:
                if (_state.CurrentInput.Length == 0) return;
                await SearchYouTubeAsync();
                break;

            case ConsoleKey.T:
                await BuildStreamAsync();
                break;

            case ConsoleKey.K:
                _state.IsTyping = true;
                _state.InputBuffer = "";
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
