using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR.UI.MenuControllers
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
        [SerializeField] private TMP_Dropdown opponentTypeDropdownMenu;

        protected override void Initialize()
        {
            playButton.onClick.AddListener(OnPlayButtonClicked);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            // subscribe to events to update the UI
            lifecycle.EventBus.SettingEvents.GridWidthChanged += OnGridWidthChanged;
            lifecycle.EventBus.SettingEvents.GridHeightChanged += OnGridHeightChanged;
            lifecycle.EventBus.SettingEvents.WinLengthChanged += OnWinLengthChanged;

            opponentTypeDropdownMenu.onValueChanged.AddListener(OnOpponentTypeChanged);

            // Set the initial values for the UI elements
            widthInfoText.SetText(lifecycle.ColumnCount.ToString());
            heightInfoText.SetText($"x {lifecycle.RowCount.ToString()}");
            winLengthInfoText.SetText($"{lifecycle.WinLength.ToString()} Tiles");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);

            opponentTypeDropdownMenu.onValueChanged.RemoveListener(OnOpponentTypeChanged);

            // unsubscribe from events
            lifecycle.EventBus.SettingEvents.GridWidthChanged -= OnGridWidthChanged;
            lifecycle.EventBus.SettingEvents.GridHeightChanged -= OnGridHeightChanged;
            lifecycle.EventBus.SettingEvents.WinLengthChanged -= OnWinLengthChanged;
        }

        private void OnPlayButtonClicked()
        {
            // Handle play button click
            _ = lifecycle.SetGameStateAsync(StateType.GameStarting);
            PanelDisabled();
        }

        private void OnSettingsButtonClicked()
        {
            // Handle settings button click
            lifecycle.EventBus.UiEvents.RequestMenuPanel(PanelType.SettingsMenu);
        }

        private void OnOpponentTypeChanged(int arg0)
        {
            // publish the event to change the opponent type
            OpponentType opponentType = (OpponentType)arg0;
            lifecycle.EventBus.SettingEvents.InvokeOpponentTypeChanged(opponentType);
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