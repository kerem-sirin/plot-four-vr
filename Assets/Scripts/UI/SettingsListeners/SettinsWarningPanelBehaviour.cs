namespace PlotFourVR.UI.SettingsListeners
{
    /// <summary>
    /// This class manages the visibility of a warning panel in the settings menu
    /// based on grid size validity.
    /// </summary>
    public class SettinsWarningPanelBehaviour : SettingsListener
    {
        protected override void OnGridSizeValidityChanged(bool isValid)
        {
            gameObject.SetActive(!isValid);
        }

        protected override void SetInitialObjectState()
        {
            gameObject.SetActive(false);
        }
    }
}