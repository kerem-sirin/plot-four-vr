using PlotFourVR.UI.SettingsListeners;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR.UI.MenuControllers
{
    /// <summary>
    /// Controller for the settings menu in the game.
    /// </summary>
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
            lifecycle.EventBus.SettingEvents.GridWidthChanged += OnGridWidthChanged;
            lifecycle.EventBus.SettingEvents.GridHeightChanged += OnGridHeightChanged;
            lifecycle.EventBus.SettingEvents.WinLengthChanged += OnWinLengthChanged;

            // Set the initial values for the UI elements
            widthSlider.Slider.value = lifecycle.ColumnCount;
            heightSlider.Slider.value = lifecycle.RowCount;
            winLengthSlider.Slider.value = lifecycle.WinLength;

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
            lifecycle.EventBus.SettingEvents.GridWidthChanged -= OnGridWidthChanged;
            lifecycle.EventBus.SettingEvents.GridHeightChanged -= OnGridHeightChanged;
            lifecycle.EventBus.SettingEvents.WinLengthChanged -= OnWinLengthChanged;
        }

        private void OnAcceptButtonClicked()
        {
            lifecycle.EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
            PanelDisabled();
        }

        private void OnResetToDefaultValuesButtonClicked()
        {
            lifecycle.EventBus.SettingEvents.InvokeGridWidthChanged(9);
            lifecycle.EventBus.SettingEvents.InvokeGridHeightChanged(7);
            lifecycle.EventBus.SettingEvents.InvokeWinLengthChanged(4);
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
            lifecycle.EventBus.SettingEvents.InvokeGridWidthChanged(width);
        }

        private void OnHeightSliderValueChanged(float arg0)
        {
            int height = (int)heightSlider.Slider.value;
            lifecycle.EventBus.SettingEvents.InvokeGridHeightChanged(height);
        }

        private void OnWinLengthSliderValueChanged(float arg0)
        {
            int winLength = (int)winLengthSlider.Slider.value;
            lifecycle.EventBus.SettingEvents.InvokeWinLengthChanged(winLength);
        }

        private void CheckGridSizeValidity()
        {
            // Check if the grid size is valid
            bool isValid = lifecycle.IsGridSizeValid();
            if (isValid != isGridSizeValid)
            {
                // if the grid size validity changes, fire event
                isGridSizeValid = isValid;
                GridSizeValidityChanged?.Invoke(isGridSizeValid);
            }
        }
    }
}