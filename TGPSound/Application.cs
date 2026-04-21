using Spectre.Console;
using TGPSound.Ui;
using TGPSound.Models;

namespace TGPSound;

public static class Application
{
    public static async Task Main()
    {
        var state = new AppState();
        var manager = new ScreenManager(state);

        await AnsiConsole.Live(manager.GetCurrentScreen().Render())
            .StartAsync(async ctx =>
            {
                while (true)
                {
                    var screen = manager.GetCurrentScreen();
                    ctx.UpdateTarget(screen.Render());

                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        if (state.IsTyping)
                        {
                            HandleTyping(key, state);
                        }
                        else
                        {
                            screen.HandleActions(key);
                        }
                    }
                }
            });
    }

    private static void HandleTyping(ConsoleKey key, AppState state)
    {
        switch (key)
        {
            case ConsoleKey.Enter:
                state.IsTyping = false;
                state.CurrentInput = state.InputBuffer.ToLower();
                state.InputBuffer = "";
                break;

            case ConsoleKey.Backspace:
                if (state.InputBuffer.Length > 0)
                    state.InputBuffer = state.InputBuffer[..^1];
                break;

            default:
                var c = GetCharFromKey(key);
                if (c != '\0')
                    state.InputBuffer += c;
                break;
        }
    }

    private static char GetCharFromKey(ConsoleKey key)
    {
        if (key >= ConsoleKey.A && key <= ConsoleKey.Z)
            return (char)key;

        if (key >= ConsoleKey.D0 && key <= ConsoleKey.D9)
            return (char)('0' + (key - ConsoleKey.D0));

        if (key == ConsoleKey.Spacebar)
            return ' ';

        return '\0';
    }
}

public class AppState
{
    public Screen CurrentScreen { get; set; } = Screen.Main;
    public MetadataItem CurrentMetadata { get; set; } = new() { Title = "Nada Tocando" };
    public string CurrentInput { get; set; } = "";
    public string InputBuffer { get; set; } = "";
    public bool IsTyping { get; set; } = false;
}


