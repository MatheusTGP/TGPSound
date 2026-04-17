using Spectre.Console;

namespace TGPSound.Ui;

static internal class Components
{
    public static Text StyledText(string text, Color? color) =>
        new(text, new Style(foreground: color));
}