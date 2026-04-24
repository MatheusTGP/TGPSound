using Spectre.Console;
using Spectre.Console.Rendering;
using TGPSound.Common;
using TGPSound.Services;
using TGPSound.Services.Responses;

namespace TGPSound.Ui.Screens;

internal class SearchUI(AppState state) : IScreen
{
    private const int MAX_SEARCH_RESULTS = 10;

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
                    new Markup("[blue]K[/] = Toggle Keyboard"),
                    new Markup("[blue]P[/] = Search"),
                    new Markup("[blue]T[/] = PlayerScreen"),
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

    private async Task BuildPlayerAsync()
    {
        var video = searchResults?[selectedItemIndex];

        _state.CurrentMetadata.Url = "";
        _state.NavigateTo(Screen.Player);

        if (video != null)
        {
            _state.SetMetadata(video.Title, video.Author, video.Duration);
            var metadata = await youtube.GetStream(video.VideoId);
            if (metadata != null)
            {
                _state.CurrentMetadata.Url = metadata.GetBestAudioUrl();
                await LoadThumbnailAsync(video.Thumbnail);
            }
            else
            {
                _state.CurrentMetadata.Title = "Fail in get streaming";
            }
        }
        else
        {
            _state.CurrentMetadata.Title = "Error from get video details";
        }
    }

    private async Task LoadThumbnailAsync(string imageUrl)
    {
        var path = Path.Combine("cache", "thumb.png");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory("cache");
        }

        var bytes = await Http.Client.GetByteArrayAsync(imageUrl);
        try
        {
            await File.WriteAllBytesAsync(path, bytes);
        }
        finally
        {
            _state.CurrentMetadata.ThumbnailPath = path;
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
    public void HandleActions(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Q:
                _state.CurrentScreen = Screen.Main;
                break;

            case ConsoleKey.P:
                _ = Task.Run(SearchYouTubeAsync);
                break;

            case ConsoleKey.T:
                _ = Task.Run(BuildPlayerAsync);
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
