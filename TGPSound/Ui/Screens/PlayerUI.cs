using LibVLCSharp.Shared;
using Spectre.Console;
using Spectre.Console.Rendering;
using TGPSound.Controllers;

namespace TGPSound.Ui.Screens;

internal class PlayerUI(AppState state) : IScreen
{
    private readonly PlayerController playerController = new(new LibVLC("--no-video", "--quiet"));
    private readonly AppState _state = state;
    private int thumbnailSize = 15;

    public IRenderable Render()
    {
        return new Panel(
            new Rows(
                new Markup(new string('\n', 2)),
                RenderThumbnail(),
                new Markup("\n"),
                new Markup($"[bold aqua]{_state.CurrentMetadata.Title}[/]").Centered(),
                new Markup($"[gray]{_state.CurrentMetadata.Artist}[/]\n").Centered(),
                RenderProgressBar(),
                new Columns(
                    new Markup($"(A)[aqua] - 5 [/]").Centered(),
                    new Markup(
                    _state.CurrentMetadata.IsPlaying
                        ? "[red] ██[/]"
                        : "[bold aqua]█ █[/]"
                    ).Centered(),
                    new Markup($"[aqua] + 5 [/](D)").Centered()
                ),
                new Markup(new string('\n', 3)),
                new Columns(
                    new Markup("[grey](L)[/] Lyrics"),
                    new Markup("[grey](E)[/] Decrease art size").Centered(),
                    new Markup("[grey](R)[/]Increase art size").Centered(),
                    new Markup("[grey](F)[/] Queue").RightJustified()
                ).Expand()
            )
        )
        .Border(BoxBorder.Rounded)
        .BorderColor(Color.Black)
        .Expand();
    }

    private Align RenderThumbnail()
    {
        if (_state.CurrentMetadata.ThumbnailPath == null)
        {
            return Align.Center(new Markup("Loading artwork..."));
        }
        return Align.Center(
            new CanvasImage(_state.CurrentMetadata.ThumbnailPath)
                .MaxWidth(thumbnailSize)
                .BilinearResampler()
        );
    }

    private Rows RenderProgressBar()
    {
        int width = 50;

        var duration = ParseDuration(_state.CurrentMetadata.Duration);
        var current = TimeSpan.FromSeconds(playerController.GetCurrentTime());
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
            ? _state.CurrentMetadata.Duration
            : "--:--";

        return new Rows(
            new Markup($"[aqua]{bar}[/]").Centered(),
            new Markup(
                $"[grey]{current:mm\\:ss}[/] {new string(' ', width - 12)} [grey]{durationText}[/]"
            ).Centered()
        );
    }

    public async Task HandleActions(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Q:
                {
                    playerController.Stop();
                    _state.CurrentScreen = Screen.Search;
                    break;
                }
                ;
            case ConsoleKey.T:
                {
                    if (!string.IsNullOrEmpty(_state.CurrentMetadata.Url))
                    {
                        playerController.SetMedia(playerController.BuildMedia(_state.CurrentMetadata.Url));
                    }
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
                    playerController.PlayPause();
                    _state.CurrentMetadata.IsPlaying = playerController.IsPlaying();
                    break;
                }
            case ConsoleKey.D: playerController.Forward(); break;
            case ConsoleKey.A: playerController.Backward(); break;
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