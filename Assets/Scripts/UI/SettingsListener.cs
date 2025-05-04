using PlotFourVR.MenuControllers;
using UnityEngine;

namespace PlotFourVR
{
    public abstract class SettingsListener : MonoBehaviour
    {
        SettingsMenuController settingsMenuController;
        public void Initialize(SettingsMenuController settingsMenuController)
        {
            this.settingsMenuController = settingsMenuController;

            // Subscribe to the GridSizeValidityChanged event
            settingsMenuController.GridSizeValidityChanged += OnGridSizeValidityChanged;

            SetInitialObjectState();
        }

        private void OnDestroy()
        {
            // Unsubscribe from the GridSizeValidityChanged event
            settingsMenuController.GridSizeValidityChanged -= OnGridSizeValidityChanged;
        }

        protected abstract void OnGridSizeValidityChanged(bool isValid);

        protected abstract void SetInitialObjectState();
    }
}