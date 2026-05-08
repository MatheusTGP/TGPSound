using LibVLCSharp.Shared;
using Spectre.Console;
using Spectre.Console.Rendering;
using TGPSound.Common;
using TGPSound.Controllers;
using TGPSound.Services;

namespace TGPSound.Ui.Screens;

internal class PlayerUI : IScreen
{
    private readonly AppState _state;
    private bool showLyrics = false;
    private bool playerReady = false;
    private int thumbnailSize = 15;

    private PlayerController? playerController = null;
    private readonly int lyricsWindowSize = 20;
    private int lyricsIndex = 0;
    private int lyricsLines = 0;

    public PlayerUI(AppState state)
    {
        _state = state;
        _ = InitVlcAsync();
    }

    public async Task InitVlcAsync()
    {
        await Task.Run(() => {
            Core.Initialize();
            playerController = new PlayerController(new LibVLC("--no-video", "--quiet"));
            playerReady = true;
        });
    }

    public IRenderable Render()
    {
        return new Panel(
            new Rows(
                new Markup(new string('\n', 2)),
                RenderLyricsOrThumb(),
                new Markup("\n"),
                new Markup($"[bold aqua]{_state.PlayerMetadata.Title}[/]").Centered(),
                new Markup($"[gray]{_state.PlayerMetadata.Artist}[/]\n").Centered(),
                RenderProgressBar(),
                new Columns(
                    new Markup($"(A)[aqua] - 5 [/]").Centered(),
                    new Markup(
                    _state.PlayerMetadata.IsPlaying
                        ? "[red] ██[/]"
                        : "[bold aqua]█ █[/]"
                    ).Centered(),
                    new Markup($"[aqua] + 5 [/](D)").Centered()
                ),
                new Markup(new string('\n', 3)),
                new Columns(
                    new Markup($"[grey](L)[/] {(showLyrics ? "Hide" : "Show")} Lyrics"),
                    new Markup("[grey](E)[/] Decrease art size").Centered(),
                    new Markup("[grey](R)[/] Increase art size").Centered(),
                    new Markup($"isReady: {playerReady}").RightJustified()
                ).Expand()
            )
        )
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Black)
        .Expand();
    }

    private Align RenderLyricsOrThumb()
    {
        if (!showLyrics || _state.PlayerMetadata.PlainLyrics == null)
        {
            return RenderThumbnail();
        }
        
        return Align.Center(
            new Markup($"[bold white]{
                LyricsHelper.GetLyricView(
                    _state.PlayerMetadata.PlainLyrics,
                    lyricsIndex,
                    lyricsWindowSize
                )
            }[/]")
        );
    }

    private Align RenderThumbnail()
    {
        if (_state.PlayerMetadata.ThumbnailPath == null)
        {
            return Align.Center(new Markup("Loading artwork..."));
        }
        return Align.Center(
            new CanvasImage(_state.PlayerMetadata.ThumbnailPath)
                .MaxWidth(thumbnailSize)
                .BilinearResampler()
        );
    }

    private Rows RenderProgressBar()
    {
        int width = 50;

        var duration = ParseDuration(_state.PlayerMetadata.Duration);
        var current = TimeSpan.FromSeconds(playerController?.GetCurrentTime() ?? 0);
        if (duration > TimeSpan.Zero && current > duration)
            current = duration;

        double progress = duration.TotalSeconds > 0
            ? current.TotalSeconds / duration.TotalSeconds
            : 0;
        progress = Math.Clamp(progress, 0, 1);

        int position = (int)(progress * width);
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < width; i++)
        {
            if (i == position)
                sb.Append("[bold white]|[/]");
            else if (i < position)
                sb.Append("[aqua]█[/]");
            else
                sb.Append("[gray]░[/]");
        }

        var bar = sb.ToString();
        string durationText = duration.TotalSeconds > 0
            ? _state.PlayerMetadata.Duration
            : "--:--";

        return new Rows(
            new Markup($"[aqua]{bar}[/]").Centered(),
            new Markup(
                $"[grey]{current:mm\\:ss}[/] {new string(' ', width - 12)} [grey]{durationText}[/]"
            ).Centered()
        );
    }

    private async Task SearchLyricsAsync()
    {
        if (_state.PlayerMetadata.PlainLyrics == null)
        {
            showLyrics = true;
            _state.SetPlainLyrics("Searching lyrics...");
            try
            {
                var lyrics = await APIsServices.GetLrclibLyrics(_state.PlayerMetadata.Title, _state.PlayerMetadata.Artist);
                if (lyrics != null)
                {
                    _state.SetPlainLyrics(lyrics.PlainLyrics);
                    lyricsLines = _state.PlayerMetadata.PlainLyrics?.Split('\n').Length ?? 0;
                }
                else
                {
                    _state.SetPlainLyrics("No lyrics found!");
                }
            }
            catch (Exception ex)
            {
                _state.SetPlainLyrics($"Error fetching lyrics:\n{ex.Message}");
            }
        }
    }

    public async Task HandleActions(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Q:
                {
                    playerController?.Stop();
                    _state.PlayerMetadata.SetLyrics(null);
                    showLyrics = false;
                    lyricsIndex = 0;
                    _state.NavigateTo(Screen.Search);
                    break;
                }
                ;
            case ConsoleKey.T:
                {
                    if (string.IsNullOrEmpty(_state.PlayerMetadata.Url) || !playerReady) return;
                    playerController?.SetMedia(playerController.BuildMedia(_state.PlayerMetadata.Url));
                    break;
                }
            case ConsoleKey.L:
                {
                    showLyrics = !showLyrics;
                    _ = SearchLyricsAsync();
                    break;
                }
            case ConsoleKey.E:
                {
                    thumbnailSize -= 10;
                    break;
                }
            case ConsoleKey.R:
                {
                    thumbnailSize += 10;
                    break;
                }
            case ConsoleKey.Spacebar:
                {
                    playerController?.PlayPause();
                    _state.PlayerMetadata.IsPlaying = playerController?.IsPlaying() ?? false;
                    break;
                }
            case ConsoleKey.UpArrow:
                {
                    lyricsIndex = LyricsHelper.ScrollUp(lyricsIndex, 1);
                    break;
                }
            case ConsoleKey.DownArrow:
                {
                    lyricsIndex = LyricsHelper.ScrollDown(
                        lyricsIndex,
                        1,
                        lyricsLines,
                        lyricsWindowSize
                    );
                    break;
                }
            case ConsoleKey.D: playerController?.Forward(); break;
            case ConsoleKey.A: playerController?.Backward(); break;
        }
    }

    private static TimeSpan ParseDuration(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return TimeSpan.Zero;

        var parts = input.Split(':');

        if (parts.Length == 2)
        {
            int minutes = int.Parse(parts[0]);
            int seconds = int.Parse(parts[1]);
            return TimeSpan.FromMinutes(minutes) + TimeSpan.FromSeconds(seconds);
        }

        if (parts.Length == 3)
        {
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);
            return new TimeSpan(hours, minutes, seconds);
        }

        return TimeSpan.Zero;
    }
}