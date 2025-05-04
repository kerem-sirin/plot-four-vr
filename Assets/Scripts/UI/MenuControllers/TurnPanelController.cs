using UnityEngine;

namespace PlotFourVR.UI.MenuControllers
{
    public class TurnPanelController : MenuController
    {
        [Header("Turn Panel Properties")]
        [SerializeField] private StateType activeState;

        protected override void Initialize()
        {
            runtimeController.GameStateChanged += OnGameStateChanged;

            runtimeController.EventBus.UiEvents.RepositionGridRelatedMenuPositioningRequested += OnRepositionGridRelatedMenuPositioningRequested;
        }

        private void OnRepositionGridRelatedMenuPositioningRequested(Vector3 newPosition)
        {
            Vector3 offset = new Vector3(-0.5f, 0.4f, 0f);
            transform.position = newPosition;
            transform.localPosition += offset;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            runtimeController.GameStateChanged -= OnGameStateChanged;

            runtimeController.EventBus.UiEvents.RepositionGridRelatedMenuPositioningRequested -= OnRepositionGridRelatedMenuPositioningRequested;
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