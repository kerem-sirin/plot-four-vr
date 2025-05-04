using UnityEngine.UI;

namespace PlotFourVR.UI.SettingsListeners
{
    /// <summary>
    /// Manages the state of the accept button in the settings menu,
    /// in case the grid size is valid or not.
    /// </summary>
    public class SettingsAcceptButtonBehaviour : SettingsListener
    {
        private Button button;

        protected override void OnGridSizeValidityChanged(bool isValid)
        {
            button.interactable = isValid;
        }

        protected override void SetInitialObjectState()
        {
            button = GetComponent<Button>();
            button.interactable = true;
        }
    }
}