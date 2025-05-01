using System;

namespace PlotFourVR
{
    public class UiEvents
    {
        public event Action<PanelType> MenuPanelRequested;
        public void RequestMenuPanel(PanelType menuType)
        {
            MenuPanelRequested?.Invoke(menuType);
        }
    }
    
    public enum PanelType
    {
        MainMenu,
        SettingsMenu,
        YellowTurnMenu,
        RedTurnMenu,
        GameOverMenu,
    }
}