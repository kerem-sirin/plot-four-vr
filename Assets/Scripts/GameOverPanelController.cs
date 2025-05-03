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

        protected override void Initialize()
        {
            playAgainButton.onClick.AddListener(OnPlayAgainButtonClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

            runtimeController.EventBus.UiEvents.RepositionGridRelatedMenuPositioningRequested += OnRepositionGridRelatedMenuPositioningRequested;
        }

        private void OnRepositionGridRelatedMenuPositioningRequested(Vector3 vector)
        {
            Vector3 offset = new Vector3(-0.55f, 0.75f, 0f);
            transform.position = vector;
            transform.localPosition += offset;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            playAgainButton.onClick.RemoveListener(OnPlayAgainButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

            runtimeController.EventBus.UiEvents.RepositionGridRelatedMenuPositioningRequested -= OnRepositionGridRelatedMenuPositioningRequested;
        }

        private void OnMainMenuButtonClicked()
        {
            runtimeController.SetCurrentState(StateType.EndingCurrentGame);
        }

        private void OnPlayAgainButtonClicked()
        {
            runtimeController.SetCurrentState(StateType.GameStarting);
        }

        protected override void PanelEnabled()
        {
            base.PanelEnabled();

            if (runtimeController.GameResult == ResultType.PlayerOneWin)
            {
                resultText.SetText("Victory for <color=yellow>Yellow!</color>");
            }
            else if (runtimeController.GameResult == ResultType.PlayerTwoWin)
            {
                resultText.SetText("Victory for <color=red>Red!</color>");
            }
            else if (runtimeController.GameResult == ResultType.Draw)
            {
                resultText.SetText("It's a Draw!");
            }

            tilesPlayedText.SetText(runtimeController.NodeParent.PlayedTileCount.ToString());
            playTimeText.SetText(FormatTime(runtimeController.NodeParent.PlayTime));
        }

        private string FormatTime(float playtime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(playtime);
            if (timeSpan.Hours < 1)
            {
                if (timeSpan.Minutes < 1)
                {
                    return string.Format("{0:D2}s", timeSpan.Seconds);
                }

                return string.Format("{0:D2}m:{1:D2}s", timeSpan.Minutes, timeSpan.Seconds);
            }

            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}