using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR
{
    public class GameOverPanelController : MenuController
    {
        // This class is responsible for the game over UI and its interactions.
        [Header("Game Over Panel Properties")]
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI tilesPlayedText;
        [SerializeField] private TextMeshProUGUI playTimeText;
        [SerializeField] private NodeParent nodeParent;

        protected override void Awake()
        {
            base.Awake();
            playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            playAgainButton.onClick.RemoveListener(OnPlayAgainButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
        }

        private void OnMainMenuButtonClicked()
        {
            RuntimeController.Instance.EventBus.UiEvents.RequestMenuPanel(PanelType.MainMenu);
        }

        private void OnPlayAgainButtonClicked()
        {
            RuntimeController.Instance.SetCurrentState(StateType.GameStarting);
            PanelDisabled();
        }

        protected override void PanelEnabled()
        {
            base.PanelEnabled();

            if (RuntimeController.Instance.GameResult == ResultType.PlayerOneWin)
            {
                resultText.SetText("Victory for <color=yellow>Yellow!</color>");
            }
            else if (RuntimeController.Instance.GameResult == ResultType.PlayerTwoWin)
            {
                resultText.SetText("Victory for <color=red>Red!</color>");
            }
            else if (RuntimeController.Instance.GameResult == ResultType.Draw)
            {
                resultText.SetText("It's a Draw!");
            }
            tilesPlayedText.SetText(nodeParent.PlayedTileCount.ToString());
            playTimeText.SetText(FormatTime(nodeParent.PlayTime));
        }

        private string FormatTime(float playtime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(playtime);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}
