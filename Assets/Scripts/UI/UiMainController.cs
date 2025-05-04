using PlotFourVR.Controllers;
using PlotFourVR.UI.MenuControllers;
using UnityEngine;

namespace PlotFourVR.UI
{
    /// <summary>
    /// Main controller for the UI, managing different menu panels and their initialization.
    /// </summary>
    public class UiMainController : MonoBehaviour
    {
        private MenuController[] menuPanels;
        public void Initialize(GameLifecycleController lifecycle)
        {
            // Initialize UI elements and event listeners here
            menuPanels = GetComponentsInChildren<MenuController>(true);

            foreach (var panel in menuPanels)
            {
                panel.Initialize(lifecycle);
            }
        }
    }

    public enum PanelType
    {
        MainMenu,
        SettingsMenu,
        YellowTurnMenu,
        RedTurnMenu,
        GameOverMenu,
        None,
        GreenTurnMenu,
    }
}