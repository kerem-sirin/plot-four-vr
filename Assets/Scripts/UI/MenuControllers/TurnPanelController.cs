using System;
using UnityEngine;

namespace PlotFourVR
{
    public class TurnPanelController : MenuController
    {
        [Header("Turn Panel Properties")]
        [SerializeField] private StateType activeState;

        protected override void Initialize()
        {
            RuntimeController.Instance.GameStateChanged += OnGameStateChanged;

            RuntimeController.Instance.EventBus.UiEvents.RepositionGridRelatedMenuPositioningRequested += OnRepositionGridRelatedMenuPositioningRequested;
        }

        private void OnRepositionGridRelatedMenuPositioningRequested(Vector3 vector)
        {
            Vector3 offset = new Vector3(-0.5f, 0.4f, 0f);
            transform.position = vector;
            transform.localPosition += offset;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RuntimeController.Instance.GameStateChanged -= OnGameStateChanged;

            RuntimeController.Instance.EventBus.UiEvents.RepositionGridRelatedMenuPositioningRequested -= OnRepositionGridRelatedMenuPositioningRequested;
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