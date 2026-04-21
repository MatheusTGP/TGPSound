using Spectre.Console;
using Spectre.Console.Rendering;
using TGPSound.Ui;

namespace TGPSound.UI.Screens;

internal class MainUI(AppState state) : IScreen
{
    private readonly AppState appState = state;
    public int selectedActionIndex = 0;

    public IRenderable Render()
    {
        return new Panel(
            new Rows(
                new Markup("\n"),
                new FigletText("TGPSound").Color(Color.Red),
                new Markup("[bold white on red]A lightweight YouTube console client built with C# and .NET[/]"),
                new Markup("[bold white]search | extract | play audio | streams | metadata\n[/][grey]directly from the terminal, thank you! [/]\n"),
                new Align(new Rows(ScreenActions.mainMenuActions), HorizontalAlignment.Right)
            )
        ).NoBorder();
    }

    public void HandleActions(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.S:
                appState.IsTyping = true;
                appState.InputBuffer = "";
                appState.CurrentScreen = Screen.Search;
                break;
                
            // More actions in future..

            case ConsoleKey.Q:
                Console.Clear();
                Environment.Exit(0);
                break;
        }
    }

    
}
