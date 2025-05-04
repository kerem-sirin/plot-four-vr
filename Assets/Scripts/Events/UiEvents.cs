using PlotFourVR.UI;
using System;
using UnityEngine;

namespace PlotFourVR.Events
{
    /// <summary>
    /// Handles UI events for the game.
    /// </summary>
    public class UiEvents
    {
        public event Action<PanelType> MenuPanelRequested;
        public void RequestMenuPanel(PanelType menuType)
        {
            MenuPanelRequested?.Invoke(menuType);
        }

        public event Action<Vector3> GridLayoutReady;
        public void InvokeGridLayoutReady(Vector3 newPosition)
        {
            GridLayoutReady?.Invoke(newPosition);
        }
    }
}