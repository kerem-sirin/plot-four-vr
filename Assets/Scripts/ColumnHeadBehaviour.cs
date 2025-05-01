using System;
using UnityEngine;

namespace PlotFourVR
{
    public class ColumnHeadBehaviour : MonoBehaviour
    {
        private const float HIGHLIGHT_SCALE = 1f; // Scale factor for highlighting
        private const float DEFAULT_SCALE = 0.1f; // Default scale for the column head

        [SerializeField] private Material defaultMaterual;
        [SerializeField] private Material redHighlightMaterial;
        [SerializeField] private Material yellowHighlightMaterial;

        private int columnIndex; // The index of the column this head belongs to
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();

            RuntimeController.Instance.EventBus.InteractionEvents.NodeHoverEntered += OnNodeHoverEntered;
            RuntimeController.Instance.EventBus.InteractionEvents.NodeHoverExited += OnNodeHoverExited;

            // Apply default visual properties
            RemoveHighlightFromColumnHead();
        }

        private void OnDestroy()
        {
            RuntimeController.Instance.EventBus.InteractionEvents.NodeHoverEntered -= OnNodeHoverEntered;
            RuntimeController.Instance.EventBus.InteractionEvents.NodeHoverExited -= OnNodeHoverExited;
        }

        private void OnNodeHoverEntered(Node node)
        {
            if (node.ColumnIndex == columnIndex)
            {
                // Highlight the column head
                HighlightColumnHead();
            }
        }

        private void OnNodeHoverExited(Node node)
        {
            if (node.ColumnIndex == columnIndex)
            {
                // Remove highlight from the column head
                RemoveHighlightFromColumnHead();
            }
        }

        private void HighlightColumnHead()
        {
            // if the current state is not of the player turns, do not highlight
            if (RuntimeController.Instance.CurrentState != StateType.PlayerOneTurn &&
                RuntimeController.Instance.CurrentState != StateType.PlayerTwoTurn) return;
            
            transform.localScale = Vector3.one * HIGHLIGHT_SCALE; // Example scaling for highlight effect

            // Set the highlight material depending on the player turn
            if (RuntimeController.Instance.CurrentState == StateType.PlayerOneTurn)
            {
                meshRenderer.material = yellowHighlightMaterial;
            }
            else if (RuntimeController.Instance.CurrentState == StateType.PlayerTwoTurn)
            {
                meshRenderer.material = redHighlightMaterial;
            }
        }

        private void RemoveHighlightFromColumnHead()
        {
            // Set the default scale for the column head
            transform.localScale = Vector3.one * DEFAULT_SCALE;
            // Set the default material
            meshRenderer.material = defaultMaterual;
        }

        public void Initialize(int columnIndex)
        {
            this.columnIndex = columnIndex;
        }
    }
}