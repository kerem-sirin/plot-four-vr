namespace PlotFourVR
{
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