using UnityEngine;

namespace PlotFourVR
{
    public class TurnPanelController : MenuController
    {
        [Header("Turn Panel Properties")]
        [SerializeField] private StateType activeState;

        protected override void Awake()
        {
            base.Awake();
            RuntimeController.Instance.GameStateChanged += OnGameStateChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RuntimeController.Instance.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(StateType newState)
        {
            if (newState == activeState)
            {
                PanelEnabled();
            }
            else
            {
                PanelDisabled();
            }
        }
    }
}