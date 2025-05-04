using PlotFourVR.Controllers;
using UnityEngine;

namespace PlotFourVR.UI.MenuControllers
{
    public class TurnPanelController : MenuController
    {
        [Header("Turn Panel Properties")]
        [SerializeField] private StateType activeState;

        protected override void Initialize()
        {
            lifecycle.GameStateChanged += OnGameStateChanged;

            lifecycle.EventBus.UiEvents.GridLayoutReady += OnGridLayoutReady;
        }

        private void OnGridLayoutReady(Vector3 newPosition)
        {
            Vector3 offset = new Vector3(-0.5f, 0.4f, 0f);
            transform.position = newPosition;
            transform.localPosition += offset;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            lifecycle.GameStateChanged -= OnGameStateChanged;

            lifecycle.EventBus.UiEvents.GridLayoutReady -= OnGridLayoutReady;
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