using Spectre.Console.Rendering;

namespace TGPSound.Ui;

public interface IScreen
{
    IRenderable Render();
    async Task HandleActions(ConsoleKey key) { }
}
