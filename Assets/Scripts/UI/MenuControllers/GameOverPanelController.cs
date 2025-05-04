using PlotFourVR.Controllers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlotFourVR.UI.MenuControllers
{
    /// <summary>
    /// Handles the game over UI and its interactions.
    /// </summary>
    public class GameOverPanelController : MenuController
    {
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

            lifecycle.EventBus.UiEvents.GridLayoutReady += OnGridLayoutReady;
        }

        private void OnGridLayoutReady(Vector3 vector)
        {
            Vector3 offset = new Vector3(-0.55f, 0.5f, 0f);
            transform.position = vector;
            transform.localPosition += offset;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            playAgainButton.onClick.RemoveListener(OnPlayAgainButtonClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

            lifecycle.EventBus.UiEvents.GridLayoutReady -= OnGridLayoutReady;
        }

        private void OnMainMenuButtonClicked()
        {
            _ = lifecycle.SetGameStateAsync(StateType.EndingCurrentGame);
        }

        private void OnPlayAgainButtonClicked()
        {
            _ = lifecycle.SetGameStateAsync(StateType.GameStarting);
        }

        protected override void PanelEnabled()
        {
            base.PanelEnabled();

            if (lifecycle.GameResult == ResultType.PlayerOneWin)
            {
                resultText.SetText("Victory for <color=yellow>Yellow</color>!");
            }
            else if (lifecycle.GameResult == ResultType.PlayerTwoWin)
            {
                resultText.SetText("Victory for <color=red>Red</color>!");
            }
            else if (lifecycle.GameResult == ResultType.PlayerThreeWin)
            {
                resultText.SetText("Victory for <color=green>Green</color>!");
            }
            else if (lifecycle.GameResult == ResultType.Draw)
            {
                resultText.SetText("It's a Draw!");
            }

            tilesPlayedText.SetText(lifecycle.GridController.PlayedCount.ToString());
            playTimeText.SetText(FormatTime(lifecycle.GridController.ElapsedTime));
        }

        private string FormatTime(float playtime)
        {
            var ts = TimeSpan.FromSeconds(playtime);

            if (ts.Hours > 0) return $"{ts.Hours:D2}h:{ts.Minutes:D2}m:{ts.Seconds:D2}s";

            if (ts.Minutes > 0) return $"{ts.Minutes:D2}m:{ts.Seconds:D2}s";

            return $"{ts.Seconds:D2}s";
        }
    }
}