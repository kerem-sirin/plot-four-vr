using UnityEngine.UI;

namespace PlotFourVR.UI.SettingsListeners
{
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