using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR
{
    public class SettingsMenuController : MenuController
    {
        [Header("Settings Menu Properties")]
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button resetToDefaultValuesButton;
        [SerializeField] private SliderBehaviour widthSlider;
        [SerializeField] private SliderBehaviour heightSlider;
        [SerializeField] private SliderBehaviour winLengthSlider;

        protected override void Initialize()
        {

            acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            resetToDefaultValuesButton.onClick.AddListener(OnResetToDefaultValuesButtonClicked);

            widthSlider.Slider.onValueChanged.AddListener(OnWidthSliderValueChanged);
            heightSlider.Slider.onValueChanged.AddListener(OnHeightSliderValueChanged);
            winLengthSlider.Slider.onValueChanged.AddListener(OnWinLengthSliderValueChanged);

            // subscribe to events to update the UI
            RuntimeController.Instance.EventBus.SettingEvents.GridWidthChanged += OnGridWidthChanged;
            RuntimeController.Instance.EventBus.SettingEvents.GridHeightChanged += OnGridHeightChanged;
            RuntimeController.Instance.EventBus.SettingEvents.WinLengthChanged += OnWinLengthChanged;

            // Set the initial values for the UI elements
            widthSlider.Slider.value = RuntimeController.Instance.ColumnCount;
            heightSlider.Slider.value = RuntimeController.Instance.RowCount;
            winLengthSlider.Slider.value = RuntimeController.Instance.WinLength;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            acceptButton.onClick.RemoveListener(OnAcceptButtonClicked);
            resetToDefaultValuesButton.onClick.RemoveListener(OnResetToDefaultValuesButtonClicked);

            widthSlider.Slider.onValueChanged.RemoveListener(OnWidthSliderValueChanged);
            heightSlider.Slider.onValueChanged.RemoveListener(OnHeightSliderValueChanged);
            winLengthSlider.Slider.onValueChanged.RemoveListener(OnWinLengthSliderValueChanged);

            // unsubscribe from events
            RuntimeController.Instance.EventBus.SettingEvents.GridWidthChanged -= OnGridWidthChanged;
            RuntimeController.Instance.EventBus.SettingEvents.GridHeightChanged -= OnGridHeightChanged;
            RuntimeController.Instance.EventBus.SettingEvents.WinLengthChanged -= OnWinLengthChanged;
        }

        private void OnAcceptButtonClicked()
        {
            RuntimeController.Instance.EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
            PanelDisabled();
        }

        private void OnResetToDefaultValuesButtonClicked()
        {
            RuntimeController.Instance.EventBus.SettingEvents.InvokeGridWidthChanged(9);
            RuntimeController.Instance.EventBus.SettingEvents.InvokeGridHeightChanged(7);
            RuntimeController.Instance.EventBus.SettingEvents.InvokeWinLengthChanged(4);
        }

        private void OnGridWidthChanged(int obj)
        {
            widthSlider.Slider.value = obj;
        }

        private void OnGridHeightChanged(int obj)
        {
            heightSlider.Slider.value = obj;
        }
        private void OnWinLengthChanged(int obj)
        {
            winLengthSlider.Slider.value = obj;
        }

        private void OnWidthSliderValueChanged(float arg0)
        {
            int width = (int)widthSlider.Slider.value;
            RuntimeController.Instance.EventBus.SettingEvents.InvokeGridWidthChanged(width);
        }

        private void OnHeightSliderValueChanged(float arg0)
        {
            int height = (int)heightSlider.Slider.value;
            RuntimeController.Instance.EventBus.SettingEvents.InvokeGridHeightChanged(height);
        }

        private void OnWinLengthSliderValueChanged(float arg0)
        {
            int winLength = (int)winLengthSlider.Slider.value;
            RuntimeController.Instance.EventBus.SettingEvents.InvokeWinLengthChanged(winLength);
        }
    }
}