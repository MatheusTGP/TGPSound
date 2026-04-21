using TGPSound.Ui.Screens;
using TGPSound.UI.Screens;

namespace TGPSound.Ui;

public class ScreenManager
{
    private readonly AppState _state;
    private readonly Dictionary<Screen, IScreen> _screens;

    public ScreenManager(AppState state)
    {
        _state = state;
        _screens = new Dictionary<Screen, IScreen> {
                { Screen.Main, new MainUI(state) },
                { Screen.Player, new PlayerUI(state) },
                { Screen.Search, new SearchUI(state) }
            };
    }

    public IScreen GetCurrentScreen()
    {
        return _screens[_state.CurrentScreen];
    }
}

public enum Screen { Main, Player, Settings, Search }

