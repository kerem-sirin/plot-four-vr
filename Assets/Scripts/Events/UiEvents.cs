using PlotFourVR.UI;
using System;
using UnityEngine;

namespace PlotFourVR.Events
{
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