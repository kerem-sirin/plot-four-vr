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

        public event Action<Vector3> RepositionGridRelatedMenuPositioningRequested;
        public void RequestRepositionGridRelatedMenuPositioning(Vector3 newPosition)
        {
            RepositionGridRelatedMenuPositioningRequested?.Invoke(newPosition);
        }
    }
}