using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace TGPSound.Ui
{
    static class ScreenActions
    {
        public static readonly Markup[] mainMenuActions = [
            new Markup(KeyGuide("Search musics", "[bold white]S[/]")),
            new Markup(KeyGuide("Search lyrics", "[bold gray]A[/]")),
            new Markup(KeyGuide("Download songs", "[bold gray]B[/]")),
            new Markup(KeyGuide("Listen with ID", "[bold gray]P[/]")),
            new Markup(KeyGuide("Documentation", "[bold gray]D[/]")),
            new Markup(KeyGuide("Settings", "[bold gray]C[/]")),
            new Markup(KeyGuide("About", "[bold gray]L[/]")),
            new Markup(KeyGuide("Close", "[bold red]Q[/]"))
        ];

        public static readonly Markup[] playerActions = [
            new Markup(KeyGuide("Search musics", "[bold white]S[/]")),
            new Markup(KeyGuide("Search lyrics", "[bold gray]A[/]")),
            new Markup(KeyGuide("Download songs", "[bold gray]B[/]")),
            new Markup(KeyGuide("Listen with ID", "[bold gray]P[/]")),
            new Markup(KeyGuide("Documentation", "[bold gray]D[/]")),
            new Markup(KeyGuide("Settings", "[bold gray]C[/]")),
            new Markup(KeyGuide("About", "[bold gray]L[/]")),
            new Markup(KeyGuide("Close", "[bold red]Q[/]"))
        ];

        public static string KeyGuide(string text, string key) => $"{text} ({key})";
    }
}
