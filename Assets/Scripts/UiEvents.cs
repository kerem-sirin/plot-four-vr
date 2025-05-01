using System;

namespace PlotFourVR
{
    public class UiEvents
    {
        public event Action<MenuType> MenuPanelRequested;
        public void RequestMenuPanel(MenuType menuType)
        {
            MenuPanelRequested?.Invoke(menuType);
        }
    }
    
    public enum MenuType
    {
        MainMenu,
        SettingsMenu,
        GameOverMenu
    }

}