using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR
{
    public class SettingsMenuController : MenuController
    {
        public event Action<bool> GridSizeValidityChanged;

        [Header("Settings Menu Properties")]
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button resetToDefaultValuesButton;
        [SerializeField] private SliderBehaviour widthSlider;
        [SerializeField] private SliderBehaviour heightSlider;
        [SerializeField] private SliderBehaviour winLengthSlider;

        private bool isGridSizeValid = true;
        private SettingsListener[] settingsListeners;

        protected override void Initialize()
        {
            acceptButton.onClick.AddListener(OnAcceptButtonClicked);
            resetToDefaultValuesButton.onClick.AddListener(OnResetToDefaultValuesButtonClicked);

            widthSlider.Slider.onValueChanged.AddListener(OnWidthSliderValueChanged);
            heightSlider.Slider.onValueChanged.AddListener(OnHeightSliderValueChanged);
            winLengthSlider.Slider.onValueChanged.AddListener(OnWinLengthSliderValueChanged);

            // subscribe to events to update the UI
            runtimeController.EventBus.SettingEvents.GridWidthChanged += OnGridWidthChanged;
            runtimeController.EventBus.SettingEvents.GridHeightChanged += OnGridHeightChanged;
            runtimeController.EventBus.SettingEvents.WinLengthChanged += OnWinLengthChanged;

            // Set the initial values for the UI elements
            widthSlider.Slider.value = runtimeController.ColumnCount;
            heightSlider.Slider.value = runtimeController.RowCount;
            winLengthSlider.Slider.value = runtimeController.WinLength;

            settingsListeners = GetComponentsInChildren<SettingsListener>();
            foreach (var listener in settingsListeners)
            {
                listener.Initialize(this);
            }
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
            runtimeController.EventBus.SettingEvents.GridWidthChanged -= OnGridWidthChanged;
            runtimeController.EventBus.SettingEvents.GridHeightChanged -= OnGridHeightChanged;
            runtimeController.EventBus.SettingEvents.WinLengthChanged -= OnWinLengthChanged;
        }

        private void OnAcceptButtonClicked()
        {
            runtimeController.EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
            PanelDisabled();
        }

        private void OnResetToDefaultValuesButtonClicked()
        {
            runtimeController.EventBus.SettingEvents.InvokeGridWidthChanged(9);
            runtimeController.EventBus.SettingEvents.InvokeGridHeightChanged(7);
            runtimeController.EventBus.SettingEvents.InvokeWinLengthChanged(4);
        }

        private void OnGridWidthChanged(int obj)
        {
            widthSlider.Slider.value = obj;
            CheckGridSizeValidity();
        }

        private void OnGridHeightChanged(int obj)
        {
            heightSlider.Slider.value = obj;
            CheckGridSizeValidity();
        }
        private void OnWinLengthChanged(int obj)
        {
            winLengthSlider.Slider.value = obj;
            CheckGridSizeValidity();
        }

        private void OnWidthSliderValueChanged(float arg0)
        {
            int width = (int)widthSlider.Slider.value;
            runtimeController.EventBus.SettingEvents.InvokeGridWidthChanged(width);
        }

        private void OnHeightSliderValueChanged(float arg0)
        {
            int height = (int)heightSlider.Slider.value;
            runtimeController.EventBus.SettingEvents.InvokeGridHeightChanged(height);
        }

        private void OnWinLengthSliderValueChanged(float arg0)
        {
            int winLength = (int)winLengthSlider.Slider.value;
            runtimeController.EventBus.SettingEvents.InvokeWinLengthChanged(winLength);
        }

        private void CheckGridSizeValidity()
        {
            // Check if the grid size is valid
            bool isValid = runtimeController.IsGridSizeValid();
            if (isValid != isGridSizeValid)
            {
                // if the grid size validity changes, fire event
                isGridSizeValid = isValid;
                GridSizeValidityChanged?.Invoke(isGridSizeValid);
            }
        }
    }
}