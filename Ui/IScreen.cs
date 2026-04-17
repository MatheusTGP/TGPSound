using Spectre.Console.Rendering;

namespace TGPSound.Ui;

public interface IScreen
{
    IRenderable Render();
    void HandleActions(ConsoleKey key);
}
