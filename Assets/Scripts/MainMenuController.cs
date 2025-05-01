using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR
{
    public class MainMenuController : MenuController
    {
        // This class is responsible for the main menu UI and its interactions.
        // It inherits from MenuController, which handles common menu functionalities.

        [Header("Settings Menu Properties")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI widthInfoText;
        [SerializeField] private TextMeshProUGUI heightInfoText;
        [SerializeField] private TextMeshProUGUI winLengthInfoText;

        protected override void Awake()
        {
            base.Awake();

            playButton.onClick.AddListener(OnPlayButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            // subscribe to events to update the UI
            RuntimeController.Instance.EventBus.SettingEvents.GridWidthChanged += OnGridWidthChanged;
            RuntimeController.Instance.EventBus.SettingEvents.GridHeightChanged += OnGridHeightChanged;
            RuntimeController.Instance.EventBus.SettingEvents.WinLengthChanged += OnWinLengthChanged;

            // Set the initial values for the UI elements
            widthInfoText.SetText(RuntimeController.Instance.ColumnCount.ToString());
            heightInfoText.SetText($"x {RuntimeController.Instance.RowCount.ToString()}");
            winLengthInfoText.SetText($"{RuntimeController.Instance.WinLength.ToString()} Tiles");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);

            // unsubscribe from events
            RuntimeController.Instance.EventBus.SettingEvents.GridWidthChanged -= OnGridWidthChanged;
            RuntimeController.Instance.EventBus.SettingEvents.GridHeightChanged -= OnGridHeightChanged;
            RuntimeController.Instance.EventBus.SettingEvents.WinLengthChanged -= OnWinLengthChanged;
        }

        private void OnPlayButtonClicked()
        {
            // Handle play button click
            RuntimeController.Instance.SetCurrentState(StateType.GameStarting);
            PanelDisabled();
        }

        private void OnSettingsButtonClicked()
        {
            // Handle settings button click
            RuntimeController.Instance.EventBus.UiEvents.RequestMenuPanel(PanelType.SettingsMenu);
        }

        private void OnGridWidthChanged(int obj)
        {
            widthInfoText.SetText(obj.ToString());
        }

        private void OnGridHeightChanged(int obj)
        {
            heightInfoText.SetText($"x {obj.ToString()}");
        }

        private void OnWinLengthChanged(int obj)
        {
            headerText.SetText($"Plot {Utility.NumberToText(obj)} VR");
            winLengthInfoText.SetText($"{obj.ToString()} Tiles");
        }
    }
}